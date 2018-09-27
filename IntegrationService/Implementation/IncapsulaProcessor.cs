using Cryptopia.Base.Logging;
using Cryptopia.Common.DataContext;
using Cryptopia.Data.DataContext;
using Cryptopia.Entity;
using Cryptopia.Infrastructure.Common.DataContext;
using Cryptopia.Infrastructure.Common.Results;
using Cryptopia.Infrastructure.Incapsula.Client;
using Cryptopia.Infrastructure.Incapsula.Common.Classes;
using Cryptopia.Infrastructure.Incapsula.Common.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cryptopia.IntegrationService.Implementation
{
	public class IncapsulaProcessor : ProcessorBase<CancellationToken>
	{
		private readonly Log _log = LoggingManager.GetLog(typeof(IncapsulaProcessor));
		private readonly IIncapsulaClient _incapsulaClient = new IncapsulaClient();
		private CancellationToken _token;
		private const int _delay = 60;

		private IMonitoringDataContextFactory MonitoringDataContextFactory { get; set; }

		public IncapsulaProcessor(CancellationToken cancelToken) : base(cancelToken)
		{
			_token = cancelToken;
			MonitoringDataContextFactory = new MonitoringDataContextFactory();
		}

		protected override Log Log
		{
			get
			{
				return _log;
			}
		}


		public override string StartLog => "[Start] - Starting processor.";
		public override string StopLog => "[Start] - Stopping processor.";

		protected override async void Process()
		{
			Log.Message(LogLevel.Info, "[Process] - Starting processor.");
			while (_isEnabled)
			{
				try
				{
					DateTime now = DateTime.UtcNow;
					Entity.SiteStatistics latestReport;
					using (var context = MonitoringDataContextFactory.CreateReadOnlyContext())
					{
						latestReport = await context.SiteStatisticsReport
							.OrderByDescending(x => x.DateCreated)
							.FirstOrDefaultNoLockAsync()
							.ConfigureAwait(false);
					}

					if (latestReport == null || latestReport.DateCreated.Date < now.Date)
					{
						Log.Message(LogLevel.Info, "[Process] - Starting process...");
						await ProcessInternal().ConfigureAwait(false);
					}

					await Task.Delay(TimeSpan.FromMinutes(_delay), _token).ConfigureAwait(false);
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

		private async Task ProcessInternal()
		{
			var result = await _incapsulaClient.GetSiteStats(Infrastructure.Incapsula.Common.Enums.StatisticsValue.All, Infrastructure.Incapsula.Common.Enums.TimeRange.Today);
			await SaveReport(result);
		}

		private async Task<IWriterResult> SaveReport(Infrastructure.Incapsula.Common.Classes.SiteStatistics incoming)
		{
			try
			{
				using (var context = MonitoringDataContextFactory.CreateContext())
				{
					Cryptopia.Entity.SiteStatistics report = new Cryptopia.Entity.SiteStatistics() { DateCreated = DateTime.UtcNow };
					context.SiteStatisticsReport.Add(report);
					await context.SaveChangesAsync();

					if (incoming != null)
					{
						if (incoming.VisitsTimeseries != null)
						{
							report.VisitSummaries = new Collection<Cryptopia.Entity.VisitSummary>();

							foreach (var visit in incoming.VisitsTimeseries)
							{
								var vs = new Cryptopia.Entity.VisitSummary
								{
									SiteStatisticsId = report.Id,
									Name = visit.Name
								};

								vs.Visits = visit.ProcessedData.Select(v => new Cryptopia.Entity.VisitDataPoint
								{
									Parent = vs,
									DateOccurred = v.Day,
									DateOccurredMilliseconds = v.Day.ToTotalMilliseconds(),
									NumberOccurred = v.Number
								}).ToList();

								report.VisitSummaries.Add(vs);
							}
						}

						if (incoming.RequestsGeoDistributionSummary != null)
						{
							var distribution = new Cryptopia.Entity.GeoDistributionSummary() { SiteStatisticsId = report.Id };
							distribution.DistributionData = incoming.RequestsGeoDistributionSummary.ProcessedData
								.Select(g => new Cryptopia.Entity.GeoDistributionDataPoint
								{
									DateOccurred = g.Day,
									DateOccurredMilliseconds = g.Day.ToTotalMilliseconds(),
									NumberOccurred = g.Number,
									Location = g.Location
								}).ToList();

							report.GeoDistribution = distribution;
						}

						if (incoming.VisitsDistributionSummary != null)
						{
							report.VisitDistribution = new Collection<Cryptopia.Entity.VisitDistributionSummary>();

							foreach (var summary in incoming.VisitsDistributionSummary)
							{
								var vds = new Cryptopia.Entity.VisitDistributionSummary
								{
									Name = summary.Name
								};

								vds.VisitDistibutionData = summary.ProcessedData.Select(v => new Cryptopia.Entity.VisitDistributionDataPoint
								{
									Parent = vds,
									DateOccurred = v.Day,
									DateOccurredMilliseconds = v.Day.ToTotalMilliseconds(),
									CountryCode = v.CountryCode,
									NumberOccurred = v.Number
								}).ToList();

								report.VisitDistribution.Add(vds);
							}
						}

						if (incoming.Caching != null)
						{
							report.CachingReport = new Cryptopia.Entity.CachingReport
							{
								SiteStatisticsId = report.Id,
								SavedRequests = incoming.Caching.SavedRequests,
								TotalRequests = incoming.Caching.TotalRequests,
								SavedBytes = incoming.Caching.SavedBytes,
								TotalBytes = incoming.Caching.TotalBytes
							};
						}

						if (incoming.CachingTimeseries != null)
						{
							report.CachingSummaries = new Collection<Cryptopia.Entity.CachingSummary>();

							foreach (var summary in incoming.CachingTimeseries)
							{
								var cs = new Cryptopia.Entity.CachingSummary
								{
									Name = summary.Name
								};

								cs.CachingData = summary.ProcessedData.Select(c => new Cryptopia.Entity.CachingDataPoint
								{
									Parent = cs,
									DateOccurred = c.Day,
									DateOccurredMilliseconds = c.Day.ToTotalMilliseconds(),
									NumberOccurred = c.Number
								}).ToList();

								report.CachingSummaries.Add(cs);
							}
						}

						if (incoming.HitsTimeseries != null)
						{
							report.HitsSummaries = new Collection<Cryptopia.Entity.HitsSummary>();

							foreach (var hit in incoming.HitsTimeseries)
							{
								var hs = new Cryptopia.Entity.HitsSummary
								{
									Name = hit.Name
								};

								hs.HitsData = hit.ProcessedData.Select(h => new Cryptopia.Entity.HitDataPoint
								{
									Parent = hs,
									DateOccurred = h.Day,
									DateOccurredMilliseconds = h.Day.ToTotalMilliseconds(),
									NumberOccurred = h.Number
								}).ToList();

								report.HitsSummaries.Add(hs);
							}
						}

						if (incoming.Threats != null)
						{
							report.Threats = new Collection<Cryptopia.Entity.Threat>();

							foreach (var threat in incoming.Threats)
							{
								report.Threats.Add(new Cryptopia.Entity.Threat
								{
									Name = threat.Name,
									Incidents = threat.Incidents,
									Status = threat.Status,
									StatusText = threat.StatusText,
									StatusTextId = threat.StatusTextId,
									FollowUp = threat.FollowUp,
									FollowupText = threat.FollowupText,
									FollowupUrl = threat.FollowupUrl
								});
							}
						}

						if (incoming.BandwidthTimeseries != null)
						{
							report.BandwidthSummaries = new Collection<Cryptopia.Entity.BandwidthSummary>();

							foreach (var summary in incoming.BandwidthTimeseries)
							{
								var bs = new Cryptopia.Entity.BandwidthSummary
								{
									Name = summary.Name
								};

								bs.BandwidthData = summary.ProcessedData.Select(b => new Cryptopia.Entity.BandwidthDataPoint
								{
									Parent = bs,
									DateOccurred = b.Day,
									DateOccurredMilliseconds = b.Day.ToTotalMilliseconds(),
									NumberOccurred = b.Number
								}).ToList();

								report.BandwidthSummaries.Add(bs);
							}
						}
					}

					var returnCode = await context.SaveChangesAsync();
					return new WriterResult(true, "Successfully created statistics report in database.");
				}
			}
			catch (Exception ex)
			{
				Log.Exception(ex.Message, ex);
			}

			return new WriterResult(false, "Something went wrong creating the statistics report in the database.");
		}
	}
}
