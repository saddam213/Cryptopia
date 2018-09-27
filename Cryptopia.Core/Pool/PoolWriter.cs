using System;
using System.Linq;
using System.Threading.Tasks;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Common.Pool;
using System.Data.Entity;
using Cryptopia.Common.DataContext;
using Cryptopia.Infrastructure.Common.Results;
using Ganss.XSS;

namespace Cryptopia.Core.Pool
{
	public class PoolWriter : IPoolWriter
	{
		public IPoolDataContextFactory PoolDataContextFactory { get; set; }

		public async Task<IWriterResult> AdminUpdatePool(AdminUpdatePoolModel model)
		{
			try
			{
				using (var context = PoolDataContextFactory.CreateContext())
				{
					var pool = await context.Pool
						.Include(x => x.Statistics)
						.Where(c => c.Id == model.Id)
						.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
					if (pool == null)
						return new WriterResult(false, "Pool not found");

					var connection = await context.Connection.FirstOrDefaultNoLockAsync(x => x.AlgoType == pool.AlgoType);
					if (connection == null)
						return new WriterResult(false, "Algorithm not found");

					if (pool.Status == Enums.PoolStatus.OK &&
					    (model.Status == Enums.PoolStatus.Maintenance || model.Status == Enums.PoolStatus.Offline))
					{
						var backupPool = await context.Pool
							.Where(x => x.AlgoType == pool.AlgoType && x.Status == Enums.PoolStatus.OK && x.Id != pool.Id)
							.OrderByDescending(x => x.Statistics.Profitability)
							.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
						var workers = await context.Worker
							.Where(x => x.AlgoType == pool.AlgoType && x.TargetPool == pool.Symbol)
							.ToListNoLockAsync().ConfigureAwait(false);
						foreach (var worker in workers)
						{
							// If the default pool is not this pool set as target
							if (connection.DefaultPool != pool.Symbol)
							{
								worker.TargetPool = connection.DefaultPool;
								continue;
							}

							// If everything is down use fist working pool
							if (backupPool != null)
							{
								worker.TargetPool = backupPool.Symbol;
								continue;
							}
							// if we are here the whole mineshaft is obviously down so do nothing
						}
					}

					pool.Status = model.Status;
					pool.StatusMessage = model.StatusMessage;
					pool.BlockTime = model.BlockTime;
					pool.IsForkCheckDisabled = model.IsForkCheckDisabled;
					pool.SpecialInstructions = model.SpecialInstructions;
					pool.WalletFee = model.WalletFee;
					pool.Statistics.BlockReward = model.BlockReward;

					await context.SaveChangesAsync().ConfigureAwait(false);
					return new WriterResult(true, "Successfully updated pool details.");
				}
			}
			catch (Exception)
			{
				return null;
			}
		}

		public async Task<IWriterResult> AdminUpdatePoolConnection(AdminUpdatePoolConnectionModel model)
		{
			var sanitizer = new HtmlSanitizer();
			using (var context = PoolDataContextFactory.CreateContext())
			{
				var connection = await context.Connection.FirstOrDefaultNoLockAsync(x => x.AlgoType == model.AlgoType);
				if (connection == null)
					return new WriterResult(false, "Algorithm not found");

				connection.FixedDiffSummary = sanitizer.Sanitize(model.FixedDiffSummary);
				connection.VarDiffHighSummary = sanitizer.Sanitize(model.VarDiffHighSummary);
				connection.VarDiffMediumSummary = sanitizer.Sanitize(model.VarDiffMediumSummary);
				connection.VarDiffLowSummary = sanitizer.Sanitize(model.VarDiffLowSummary);
				connection.Name = model.Name;
				connection.Host = model.Host;
				connection.DefaultDiff = model.DefaultDiff;
				connection.DefaultPool = model.DefaultPool;

				await context.SaveChangesAsync().ConfigureAwait(false);
				return new WriterResult(true, "Successfully updated connection.");
			}
		}

		public async Task<IWriterResult> AdminUpdatePoolSettings(AdminUpdatePoolSettingsModel model)
		{
			using (var context = PoolDataContextFactory.CreateContext())
			{
				var settings = await context.Settings.FirstOrDefaultNoLockAsync().ConfigureAwait(false);
				if (settings == null)
					return new WriterResult(false, "Settings not found");

				settings.ProcessorEnabled = model.ProcessorEnabled;
				settings.HashRateCalculationPeriod = model.HashRateCalculationPeriod;
				settings.StatisticsPollPeriod = model.StatisticsPollPeriod;
				settings.PayoutPollPeriod = model.PayoutPollPeriod;
				settings.SitePayoutPollPeriod = model.SitePayoutPollPeriod;
				settings.ProfitabilityPollPeriod = model.ProfitabilityPollPeriod;
				settings.ProfitSwitchEnabled = model.ProfitSwitchEnabled;
				settings.ProfitSwitchDepthBTC = model.ProfitSwitchDepthBTC;
				settings.ProfitSwitchDepthLTC = model.ProfitSwitchDepthLTC;

				await context.SaveChangesAsync().ConfigureAwait(false);
				return new WriterResult(true, "Successfully updated settings.");
			}
		}
	}
}