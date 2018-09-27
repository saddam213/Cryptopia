using System;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Cryptopia.Base.Logging;
using Cryptopia.Common.DataContext;
using Cryptopia.Data.DataContext;
using Cryptopia.Enums;
using System.Collections.Generic;
using Cryptopia.Entity;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.IntegrationService.TradeService;

namespace Cryptopia.IntegrationService.Implementation
{
	public class ReferralProcessor : ProcessorBase<CancellationToken>
    {
        private readonly Log _log = LoggingManager.GetLog(typeof(ReferralProcessor));
		private int _pollPeriod = 12 * 60;
		private CancellationToken _cancelToken;

		public ReferralProcessor(CancellationToken cancelToken) : base(cancelToken)
        {
#if DEBUG
			_pollPeriod = 1;
#endif
			_cancelToken = cancelToken;
			DataContextFactory = new DataContextFactory();
			ExchangeDataContextFactory = new ExchangeDataContextFactory();
		}

        protected override Log Log
        {
            get
            {
                return _log;
            }
        }

        public IDataContextFactory DataContextFactory { get; set; }
		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

        public override string StartLog => "[Start] - Starting Referral processor.";
        public override string StopLog => "[Start] - Stopping Referral processor.";

        protected override async void Process()
		{
			Log.Message(LogLevel.Info, "[Process] - Starting Referral processor.");
			while (_isEnabled)
			{
				try
				{
					await ProcessRound().ConfigureAwait(false);
					await Task.Delay(TimeSpan.FromMinutes(_pollPeriod), _cancelToken).ConfigureAwait(false);
				}
				catch (TaskCanceledException)
				{
					Log.Message(LogLevel.Info, "[Process] - Referral processing canceled");
					break;
				}
			}
			_isRunning = false;
			Log.Message(LogLevel.Info, "[Process] - Stopped Referral processor.");
		}

		private async Task ProcessRound()
		{
			Log.Message(LogLevel.Info, "[ProcessRound] - Process round...");
			var currentRound = Convert.ToInt32($"{DateTime.UtcNow.Year}{DateTime.UtcNow.Month}");
			using (var context = DataContextFactory.CreateContext())
			{
				var settings = await context.Settings.FirstOrDefaultNoLockAsync();
				currentRound = settings.ReferralRound;
			}

			Log.Message(LogLevel.Info, $"[ProcessRound] - Processing round {currentRound}...");
			await ProcessReferral(currentRound);
			if (await HasRoundUpdated())
			{
				Log.Message(LogLevel.Info, "[ProcessRound] - Processing payouts...");
				await ProcessReferralPayouts(currentRound);
				Log.Message(LogLevel.Info, "[ProcessRound] - Processing payouts complete.");
			}
			Log.Message(LogLevel.Info, $"[ProcessRound] - Processing round {currentRound} complete.");
		}


		private async Task ProcessReferral(int roundId)
		{
			try
			{
				var endTime = GetRoundEndDate(roundId);
				var beginTime = GetRoundStartDate(roundId);
				List<IGrouping<string, ApplicationUser>> referralGroups;
				Log.Message(LogLevel.Info, $"[ProcessReferral] - Processing referrals for period round {roundId} ({beginTime} - {endTime})");
				using (var context = DataContextFactory.CreateContext())
				{
					referralGroups = await context.Users
						.Where(x => x.Referrer != "System")
						.GroupBy(x => x.Referrer)
						.ToListNoLockAsync();
				}

				Log.Message(LogLevel.Info, $"[ProcessReferral] - {referralGroups.Count()} referral users found, processing...");
				using (var context = ExchangeDataContextFactory.CreateContext())
				{
					var tradepairs = await context.TradePair.Where(x => x.CurrencyId1 == Constant.DOTCOIN_ID).ToListNoLockAsync();
					var tradeHistoryGroups = await context.TradeHistory
						.Where(x => x.Timestamp >= beginTime && x.Timestamp <= endTime)
						.Select(x => new
						{
							BaseId = x.TradePair.CurrencyId2,
							UserId = x.UserId,
							ToUserId = x.ToUserId,
							Fee = x.Fee
						})
						.GroupBy(x => x.BaseId)
						.ToListNoLockAsync();
					Log.Message(LogLevel.Info, $"[ProcessReferral] - {tradeHistoryGroups.Count()} referral users found, processing...");

					foreach (var referralGroup in referralGroups)
					{
						var tradeFees = 0m;
						//var activeFees = 0m;
						var userName = referralGroup.Key;
						var userIds = referralGroup.Select(x => new Guid(x.Id)).ToList();
						var user = await context.Users.FirstOrDefaultNoLockAsync(x => x.UserName == userName);
						if (user == null)
							continue;


							var processedUsers = new List<Guid>();
						foreach (var tradeHistoryGroup in tradeHistoryGroups)
						{
							var totalfee = 0m;
							foreach (var userId in userIds)
							{
								var fee = tradeHistoryGroup.Where(x => x.UserId == userId || x.ToUserId == userId).Sum(x => x.Fee);
								if (fee == 0)
								{
									//Log.Message(LogLevel.Info, $"[ProcessReferral] - No fees for found for round, Base: {tradeHistoryGroup.Key}, UserId: {userId}");
									continue;
								}

								totalfee += fee;
								processedUsers.Add(userId);
								Log.Message(LogLevel.Info, $"[ProcessReferral] - Fees for found for round, Base: {tradeHistoryGroup.Key}, Fees: {fee}, UserId: {userId}");
							}

							// convert fee to DOT
							var tradepair = tradepairs.FirstOrDefault(x => x.CurrencyId2 == tradeHistoryGroup.Key);
							if (tradepair != null && tradepair.LastTrade > 0 && totalfee > 0)
							{
								var tfee = ((totalfee / tradepair.LastTrade) / 100m) * Constant.REFERRAL_TRADEPERCENT;
								tradeFees += tfee;
								Log.Message(LogLevel.Info, $"[ProcessReferral] - Trade fees for user calculated, Base Market: {tradepair.CurrencyId2}, Fee: {totalfee}, Total DOT: {tfee}");
							}
						}

						
						var refInfo = await context.ReferralInfo.FirstOrDefaultNoLockAsync(x => x.RoundId == roundId && x.UserId == user.Id);
						if (refInfo == null)
						{
							refInfo = new ReferralInfo
							{
								UserId = user.Id,
								RoundId = roundId,
								Status = ReferralStatus.Active,
								TransferId = 0,
								Timestamp = DateTime.UtcNow
							};
							context.ReferralInfo.Add(refInfo);
						}

						if (refInfo.Status == ReferralStatus.Active)
						{
							refInfo.LastUpdate = DateTime.UtcNow;
							refInfo.RefCount = referralGroup.Count();
							refInfo.ActivityAmount = 0;// userIds.Count * Constant.REFERRAL_ACTIVEBONUS;
							refInfo.TradeFeeAmount = tradeFees;
							Log.Message(LogLevel.Info, $"[ProcessReferral] - Updated referral information for user {user.UserName}, Refs: {refInfo.RefCount}, Activity: {refInfo.ActivityAmount}, TradeFee: {refInfo.TradeFeeAmount}");
						}
					}

					Log.Message(LogLevel.Info, "[ProcessReferral] - Saving changes");
					await context.SaveChangesAsync();
					Log.Message(LogLevel.Info, "[ProcessReferral] - Changes saved");
				}

			}
			catch (Exception ex)
			{
				Log.Exception("[ProcessReferral] - An exception occurred processing Referral items.", ex);
			}
		}

		private async Task ProcessReferralPayouts(int roundId)
		{
			try
			{
				var referralsToProcess = new List<int>();
				using (var context = ExchangeDataContextFactory.CreateContext())
				{
					Log.Message(LogLevel.Info, $"[ProcessReferralPayouts] - Processing payouts for round {roundId}");
					var referrals = await context.ReferralInfo
						.Where(x => x.RoundId == roundId && x.Status == ReferralStatus.Active && x.TransferId == 0)
						.ToListNoLockAsync();
					if (referrals.Any())
					{
						foreach (var referralInfo in referrals)
						{
							referralsToProcess.Add(referralInfo.Id);
							referralInfo.Status = ReferralStatus.Processing;
						}
						await context.SaveChangesAsync();
					}
				}

				if (!referralsToProcess.Any())
				{
					Log.Message(LogLevel.Info, "[ProcessReferralPayouts] - No referral payouts found.");
					return;
				}

				Log.Message(LogLevel.Info, $"[ProcessReferralPayouts] - {referralsToProcess.Count} referrals found to process...");
				using (var tradeService = new TradeProcessorClient())
				using (var context = ExchangeDataContextFactory.CreateContext())
				{
					foreach (var referralId in referralsToProcess)
					{
						var referralInfo = await context.ReferralInfo.FirstOrDefaultNoLockAsync(x => x.Id == referralId && x.Status == ReferralStatus.Processing && x.TransferId == 0);
						if (referralInfo == null)
						{
							Log.Message(LogLevel.Info, $"[ProcessReferralPayouts] - No referral info found, Id: {referralId}");
							continue;
						}

						var amount = referralInfo.TradeFeeAmount;
						if (amount <= 0)
						{
							Log.Message(LogLevel.Info, $"[ProcessReferralPayouts] - No payout required for user, UserId: {referralInfo.UserId}");
							referralInfo.LastUpdate = DateTime.UtcNow;
							referralInfo.Status = ReferralStatus.Complete;
							continue;
						}

						Log.Message(LogLevel.Info, $"[ProcessReferralPayouts] - Processing referral transfer, UserId: {referralInfo.UserId}, Round: {referralInfo.RoundId}, Amount: {amount}");
						var response = await tradeService.SubmitTransferAsync(new SubmitTransferRequest
						{
							UserTo = referralInfo.UserId,
							UserId = Constant.SYSTEM_USER_REFERRAL,
							TransferType = TransferType.ReferralBonus,
							CurrencyId = Constant.DOTCOIN_ID,
							Amount = amount,
							NotificationTitle = "Referral Bonus!",
							NotificationMessage = "New referral bonus payment received."
						});

						if (!string.IsNullOrEmpty(response.Error))
						{
							Log.Message(LogLevel.Error, $"[ProcessReferralPayouts] - Failed to transfer funds to user, Error: {response.Error}");
							continue;
						}

						referralInfo.LastUpdate = DateTime.UtcNow;
						referralInfo.Status = ReferralStatus.Complete;
						referralInfo.TransferId = response.TransferId;
						Log.Message(LogLevel.Info, "[ProcessReferralPayouts] - Processing referral complete.");
					}

					Log.Message(LogLevel.Info, "[ProcessReferralPayouts] - Saving changes..");
					await context.SaveChangesAsync();
					Log.Message(LogLevel.Info, "[ProcessReferralPayouts] - Referral processing complete.");
				}
			}
			catch (Exception ex)
			{
				Log.Exception("[ProcessReferralPayouts] - An exception occurred processing Referral items.", ex);
			}
		}

		public async Task<bool> HasRoundUpdated()
		{
			var currentRound = Convert.ToInt32($"{DateTime.UtcNow.Year}{DateTime.UtcNow.Month}");
			using (var context = DataContextFactory.CreateContext())
			{
				var settings = await context.Settings.FirstOrDefaultNoLockAsync();
				if (currentRound > settings.ReferralRound)
				{
					Log.Message(LogLevel.Info, "[HasRoundUpdated] - New round found, updating...");
					settings.ReferralRound = currentRound;
					await context.SaveChangesAsync();
					Log.Message(LogLevel.Info, $"[HasRoundUpdated] - Round updated, Round: {currentRound}");
					return true;
				}
				Log.Message(LogLevel.Info, "[HasRoundUpdated] - No round changes found.");
			}
			return false;
		}

		private static int GetRoundYear(int round)
		{
			return Convert.ToInt32(round.ToString().Substring(0, 4));
		}

		private static int GetRoundMonth(int round)
		{
			return Convert.ToInt32(round.ToString().Substring(4));
		}

		private static DateTime GetRoundStartDate(int round)
		{
			return new DateTime(GetRoundYear(round), GetRoundMonth(round), 1);
		}

		private static DateTime GetRoundEndDate(int round)
		{
			return new DateTime(GetRoundYear(round), GetRoundMonth(round), 1).AddMonths(1);
		}
	}
}