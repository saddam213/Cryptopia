using System.Collections.Generic;
using System.Runtime.Serialization;

using Cryptopia.Infrastructure.Incapsula.Common.Classes.Stats;
using Cryptopia.Infrastructure.Incapsula.Common.Responses;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes
{
	[DataContract]
	public class SiteStatistics : ResponseData
	{
		public SiteStatistics(SiteStatisticsResponse response) : base(response)
		{
			if (response != null)
			{
				VisitsTimeseries = response.VisitsTimeseries;
				RequestsGeoDistributionSummary = response.RequestsGeoDistributionSummary;
				VisitsDistributionSummary = response.VisitsDistributionSummary;
				Caching = response.Caching;
				CachingTimeseries = response.CachingTimeseries;
				HitsTimeseries = response.HitsTimeseries;
				BandwidthTimeseries = response.BandwidthTimeseries;
				Threats = response.Threats;
				IncapRulesTimeseries = response.IncapRulesTimeseries;
			}
		}

		[DataMember]
		public List<VisitsTimeserie> VisitsTimeseries { get; set; }

		[DataMember]
		public GeoDistributionSummary RequestsGeoDistributionSummary { get; set; }

		[DataMember]
		public List<VisitsDistributionSummary> VisitsDistributionSummary { get; set; }

		[DataMember]
		public Caching Caching { get; set; }

		[DataMember]
		public List<CachingTimeserie> CachingTimeseries { get; set; }

		[DataMember]
		public List<HitsTimeserie> HitsTimeseries { get; set; }

		[DataMember]
		public List<BandwidthTimeserie> BandwidthTimeseries { get; set; }

		[DataMember]
		public List<Threat> Threats { get; set; }

		[DataMember]
		public List<object> IncapRulesTimeseries { get; set; }
	}
}
