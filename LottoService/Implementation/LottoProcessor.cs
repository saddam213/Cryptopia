using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cryptopia.Enums;
using Cryptopia.LottoService.TradeService;
using Cryptopia.Common.DataContext;
using Cryptopia.Data.DataContext;
using Cryptopia.Base.Logging;
using Cryptopia.Base;
using Cryptopia.Entity;
using Cryptopia.Common.Extensions;
using Cryptopia.Infrastructure.Common.DataContext;

namespace Cryptopia.LottoService.Implementation
{
	public class LottoProcessor
	{
		#region Fields

		private readonly Log Log = LoggingManager.GetLog(typeof(LottoProcessor));
		private bool _isEnabled;
		private bool _isRunning;
		private CancellationToken _cancelToken;
		private Random _random = new Random();

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="LottoProcessor"/> class.
		/// </summary>
		/// <param name="cancelToken">The cancel token.</param>
		public LottoProcessor(CancellationToken cancelToken)
		{
			_cancelToken = cancelToken;
			ExchangeDataContextFactory = new ExchangeDataContextFactory();
		}

		#endregion

		#region Properties

		public IExchangeDataContextFactory ExchangeDataContextFactory { get; set; }

		public bool Running
		{
			get { return _isRunning; }
		}

		#endregion

		#region Process

		/// <summary>
		/// Starts this instance.
		/// </summary>
		public void Start()
		{
			if (_isEnabled)
			{
				return;
			}

			Log.Message(LogLevel.Info, "[Start] - Starting processor.");
			_isRunning = true;
			_isEnabled = true;

			Process();
		}

		/// <summary>
		/// Stops this instance.
		/// </summary>
		public void Stop()
		{
			Log.Message(LogLevel.Info, "[Stop] - Stopping processor.");
			_isEnabled = false;
		}

		/// <summary>
		/// Processes this instance.
		/// </summary>
		private async void Process()
		{
			while (_isEnabled)
			{
				try
				{
					await ProcessLotto().ConfigureAwait(false);
					await Task.Delay(TimeSpan.FromSeconds(60), _cancelToken).ConfigureAwait(false);
				}
				catch (TaskCanceledException)
				{
					Log.Message(LogLevel.Info, "[Process] - Processing canceled");
					break;
				}
			}
			_isRunning = false;
			Log.Message(LogLevel.Info, "[Process] - Stopped processor.");
		}

		/// <summary>
		/// Processes the lotto items.
		/// </summary>
		private async Task ProcessLotto()
		{
			try
			{
				var payouts = new List<PrizePayout>();
				using (var context = ExchangeDataContextFactory.CreateContext())
				{
					Log.Message(LogLevel.Info, "[ProcessLotto] - Processing LottoItems...");
					var dueLottoDraws = await context.LottoItem.Where(x => x.Status != LottoItemStatus.Disabled && DateTime.UtcNow >= x.NextDraw).ToListNoLockAsync();
					if (dueLottoDraws.IsNullOrEmpty())
					{
						// no draws ready
						Log.Message(LogLevel.Info, "[ProcessLotto] - No lotto draws are due, Waiting...");
						return;
					}

					Log.Message(LogLevel.Info, "[ProcessLotto] - {0} lotto draws found.", dueLottoDraws.Count());
					foreach (var lottoDraw in dueLottoDraws)
					{
						Log.Message(LogLevel.Info, "[ProcessLotto] - Processing LottoItem, Id: {0}, Name: {1}, Currency: {2}", lottoDraw.Id, lottoDraw.Name, lottoDraw.CurrencyId);
						// 1. sum up total tickets, subtract site fee
						var tickets = await context.LottoTicket.Where(x => !x.IsArchived && x.LottoItemId == lottoDraw.Id && x.DrawId == lottoDraw.CurrentDrawId).ToListNoLockAsync();
						if (!tickets.IsNullOrEmpty())
						{
							Log.Message(LogLevel.Info, "[ProcessLotto] - {0} tickets found for draw.", tickets.Count());
							// Archive all the tickets
							foreach (var ticket in tickets)
								ticket.IsArchived = true;

							// Calculate prize pool
							var totalAmount = lottoDraw.Rate * tickets.Count();
							var siteFee = (totalAmount / 100m) * lottoDraw.Fee;
							var prizePool = totalAmount - siteFee;
							var prizePoolFraction = prizePool / 100m;
							var prizeWeights = LottoHelpers.GetPrizeWeights(lottoDraw.Prizes);
							var winningTicketIds = GetWinningLottoItemIds(tickets.Select(x => x.Id), lottoDraw.Prizes);
							Log.Message(LogLevel.Info, "[ProcessLotto] - LottoItem draw info, Total: {0}, PrizePool: {1}, Fee: {2}", totalAmount, prizePool, siteFee);

							// Calculate user prizes
							var drawTime = lottoDraw.NextDraw;
							for (int i = 0; i < prizeWeights.Count; i++)
							{
								var prizeWeight = prizeWeights[i];
								var winningTicket = tickets.FirstOrDefault(x => x.Id == winningTicketIds[i]);
								var amount = Math.Round(prizePoolFraction * prizeWeight, 8);
								payouts.Add(new PrizePayout
								{
									UserId = winningTicket.UserId,
									CurrencyId = lottoDraw.CurrencyId,
									Amount = amount
								});
								context.LottoHistory.Add(new LottoHistory
								{
									Amount = amount,
									Percent = prizeWeight,
									LottoItemId = lottoDraw.Id,
									Position = (i + 1),
									Timestamp = drawTime,
									UserId = winningTicket.UserId,
									LottoTicketId = winningTicket.Id,
									LottoDrawId = lottoDraw.CurrentDrawId,
									TotalAmount = prizePool
								});
								Log.Message(LogLevel.Info, "[ProcessLotto] - User payout info, UserId: {0}, Position: {1}, Weight: {2}, Amount: {3}", winningTicket.UserId, (i + 1), prizeWeight, amount);
							}
						}
						else
						{
							Log.Message(LogLevel.Info, "[ProcessLotto] - No tickets found for draw.");
						}

						// 5. update LottoItem
						lottoDraw.Status = LottoItemStatus.Finished;
						if (lottoDraw.LottoType == LottoType.Recurring || (lottoDraw.LottoType == LottoType.RecurringExpire && lottoDraw.Expires > DateTime.UtcNow))
						{
							lottoDraw.Status = LottoItemStatus.Active;
							lottoDraw.CurrentDrawId = lottoDraw.CurrentDrawId + 1;
							lottoDraw.NextDraw = lottoDraw.NextDraw.AddHours(lottoDraw.Hours);
							Log.Message(LogLevel.Info, "[ProcessLotto] - Set next draw date for recurring lotto, NextDraw: {0}", lottoDraw.NextDraw);
						}

						Log.Message(LogLevel.Info, "[ProcessLotto] - Processing LottoItem complete.");
					}

					// commit the transaction
					Log.Message(LogLevel.Debug, "[ProcessLotto] - Comitting database transaction...");
					await context.SaveChangesAsync();
					Log.Message(LogLevel.Debug, "[ProcessLotto] - Comitted database transaction.");
				}

				// Send Winnings
				await PayoutUsers(payouts);
				Log.Message(LogLevel.Info, "[ProcessLotto] - Processing LottoItems complete.");
			}
			catch (Exception ex)
			{
				Log.Exception("[ProcessLotto] - An exception occured processing LottoItem.", ex);
			}
		}

		#endregion

		private async Task PayoutUsers(IEnumerable<PrizePayout> payouts)
		{
			Log.Message(LogLevel.Info, "[PayoutUsers] - Processing user payouts.");
			if (payouts.IsNullOrEmpty())
			{
				Log.Message(LogLevel.Info, "[PayoutUsers] - No users found to payout.");
				return;
			}

			foreach (var payout in payouts)
			{
				try
				{
					Log.Message(LogLevel.Info, "[PayoutUsers] - Processing user payout, UserId: {0}, CurrncyId: {1}, Amount: {2}", payout.UserId, payout.CurrencyId, payout.Amount);
					using (var tradeService = new TradeProcessorClient())
					{
						var result = await tradeService.SubmitTransferAsync(new SubmitTransferRequest
						{
							Amount = payout.Amount,
							CurrencyId = payout.CurrencyId,
							IsApi = false,
							TransferType = TransferType.Lotto,
							UserId = Constant.SYSTEM_USER_LOTTO,
							UserTo = payout.UserId
						});

						if (!string.IsNullOrEmpty(result.Error))
						{
							Log.Message(LogLevel.Error, "[PayoutUsers] - Failed to payout user, Error: {0}", result.Error);
							continue;
						}
					}
					Log.Message(LogLevel.Info, "[PayoutUsers] -  Processing user payout complete, UserId: {0}", payout.UserId);
				}
				catch (Exception ex)
				{
					Log.Exception("[PayoutUsers] - An exception occured processing payout.", ex);
				}
			}

			Log.Message(LogLevel.Info, "[PayoutUsers] - User payouts complete.");
		}

		private List<int> GetWinningLottoItemIds(IEnumerable<int> lottoTicketIds, int prizeCount)
		{
			var result = new List<int>();
			var tickets = lottoTicketIds.ToList();
			while (tickets.Count < prizeCount)
			{
				tickets.AddRange(tickets);
			}

			for (int i = 0; i < prizeCount; i++)
			{
				var winner = _random.Next(tickets.Count);
				result.Add(tickets[winner]);
				tickets.RemoveAt(winner);
			}
			return result;
		}
	}
}