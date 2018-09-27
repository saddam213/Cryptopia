
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

using Cryptopia.Infrastructure.Incapsula.Common.Requests;
using Cryptopia.Infrastructure.Incapsula.Common.Responses;
using Cryptopia.Infrastructure.Incapsula.Common.Classes;
using Cryptopia.Infrastructure.Incapsula.Common.Enums;
using Cryptopia.Infrastructure.Incapsula.Common.Extensions;
using Cryptopia.Infrastructure.Incapsula.Common.AppConfiguration;

using Enums = Cryptopia.Infrastructure.Incapsula.Common.Enums;

namespace Cryptopia.Infrastructure.Incapsula.Client
{
    public class IncapsulaClient : IIncapsulaClient
    {
        private static readonly HttpClient _serviceClient = new HttpClient();
        private IApplicationConfiguration _config = null;
        private TargetSite _targetSite = TargetSite.None;

        public IncapsulaClient()
        {
            IApplicationConfiguration config = new ApplicationConfiguration();
            Initialize(config);
        }

        public IncapsulaClient(IApplicationConfiguration config)
        {
            Initialize(config);
        }

        #region Configuration

        public async Task<IApplicationConfiguration> GetClientConfiguration()
        {
            if (_config == null)
                return null;

            return await Task.FromResult(_config);
        }

        #endregion

        #region Status

        public async Task<SiteStatus> GetSiteStatus()
        {
            return await SendSiteStatusRequest();
        }

        #endregion

        #region Domain Get Methods

        public async Task<DomainApproverInformation> GetDomainApproverEmailAddresses()
        {
            var result = await PerformaAPIRequest(new DomainAproverEmailAddressesRequest(_config));
            var response = JsonConvert.DeserializeObject<DomainApproverInformationResponse>(result);
            return new DomainApproverInformation(response);
        }

        #endregion

        #region Site List

        public async Task<SiteListInformation> GetSiteList()
        {
            string result = await PerformaAPIRequest(new SiteListRequest(_config));
            var response = JsonConvert.DeserializeObject<SiteListInformationResponse>(result);
            return new SiteListInformation(response);
        }

        #endregion

        #region Site Reports

        public async Task<SiteReport> GetSiteReport(TimeRange range)
        {
            return await SendSiteReportRequest(range);
        }

        public async Task<SiteReport> GetSiteReport(DateTime startDate, DateTime endDate)
        {
            return await SendSiteReportRequest(startDate, endDate);
        }

        #endregion

        #region Rules

        public async Task<SiteRules> GetSiteRules()
        {
            return await SendSiteRulesRequest();
        }

        public async Task<SiteRules> GetAccountRules()
        {
            var result = await PerformaAPIRequest(new AccountRulesRequest(_config));
            return JsonConvert.DeserializeObject<SiteRules>(result);
        }

        #endregion

        #region Data Centres

        public async Task<List<DataCenters>> GetDataCenterList()
        {
            return await SendDataCenterListRequest();
        }

        #endregion

        #region Statistics

        public async Task<SiteStatistics> GetSiteStats(StatisticsValue stats, TimeRange range)
        {
            var siteSatistics = await SendSiteStatsRequest(stats, range);
            return siteSatistics;
        }

        public async Task<SiteStatistics> GetSiteStats(StatisticsValue stats, DateTime rangeStart, DateTime rangeEnd)
        {
            return await SendSiteStatsRequest(stats, rangeStart, rangeEnd);
        }

        public async Task<SiteVisits> GetSiteVisits(TimeRange range)
        {
            return await SendSiteVisitsRequest(range);
        }

        public async Task<SiteVisits> GetSiteVisits(DateTime rangeStart, DateTime rangeEnd)
        {
            return await SendSiteVisitsRequest(rangeStart, rangeEnd);
        }

        #endregion

        #region Access Control

        public async Task<IPBlacklist> BlacklistIp(string ip)
        {
            return await SendBlacklistIpRequest(ip);
        }

        public async Task<IPBlacklist> GetIpBlacklist()
        {
            return await SendGetIpBlacklistRequest();
        }

        #endregion

        #region Site Resource Management

        public async Task<ResponseCode> PurgeSiteCache()
        {
            return await SendSiteCachePurgeRequest();
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        { }

        #endregion

        #region Private Methods

        private void Initialize(IApplicationConfiguration config)
        {
            if (config != null)
            {
                _config = config;
                _targetSite = _config.TargetWebsite;
                var result = PerformaAPIRequest(new RequestBase(_config));
            }
        }

        private async Task<SiteStatus> SendSiteStatusRequest()
        {
            string result = await PerformaAPIRequest(new SiteStatusRequest(_config));
            var response = JsonConvert.DeserializeObject<SiteStatusResponse>(result);
            return new SiteStatus(response);
        }

        private async Task<SiteReport> SendSiteReportRequest(TimeRange range)
        {
            var result = await PerformaAPIRequest(new SiteReportRequest(_config, range));
            var response = JsonConvert.DeserializeObject<SiteReportResponse>(result);
            return new SiteReport(response);
        }

        private async Task<SiteReport> SendSiteReportRequest(DateTime startDate, DateTime endDate)
        {
            var result = await PerformaAPIRequest(new SiteReportRequest(_config, startDate, endDate));
            var response = JsonConvert.DeserializeObject<SiteReportResponse>(result);
            return new SiteReport(response);
        }

        private async Task<SiteRules> SendSiteRulesRequest()
        {
            var result = await PerformaAPIRequest(new SiteRulesRequest(_config));
            return JsonConvert.DeserializeObject<SiteRules>(result);
        }

        private async Task<List<DataCenters>> SendDataCenterListRequest()
        {
            var result = await PerformaAPIRequest(new DataCenterListRequest(_config));
            DataCenterListResponse response = JsonConvert.DeserializeObject<DataCenterListResponse>(result);
            return response.DataCenters;
        }

        private async Task<SiteStatistics> SendSiteStatsRequest(StatisticsValue stats, TimeRange range)
        {
            var result = await PerformaAPIRequest(new SiteStatisticsRequest(_config, range, stats));

			SiteStatisticsResponse response = null;
			try
			{
				response = JsonConvert.DeserializeObject<SiteStatisticsResponse>(result);
			}
			catch (Exception) { }

			return new SiteStatistics(response);
        }

        private async Task<SiteStatistics> SendSiteStatsRequest(StatisticsValue stats, DateTime rangeStart, DateTime rangeEnd)
        {
            var result = await PerformaAPIRequest(new SiteStatisticsRequest(_config, rangeStart, rangeEnd, stats));
            var response = JsonConvert.DeserializeObject<SiteStatisticsResponse>(result);
            return new SiteStatistics(response);
        }

        private async Task<SiteVisits> SendSiteVisitsRequest(TimeRange range)
        {
            var result = await PerformaAPIRequest(new SiteVisitsRequest(_config, range));
            var response = JsonConvert.DeserializeObject<SiteVisitsResponse>(result);
            return new SiteVisits(response);
        }

        private async Task<SiteVisits> SendSiteVisitsRequest(DateTime rangeStart, DateTime rangeEnd)
        {
            var result = await PerformaAPIRequest(new SiteVisitsRequest(_config, rangeStart, rangeEnd));
            var response = JsonConvert.DeserializeObject<SiteVisitsResponse>(result);
            return new SiteVisits(response);
        }

        private async Task<IPBlacklist> SendBlacklistIpRequest(string ip)
        {
            if (!string.IsNullOrEmpty(ip))
            {
                // requests seem to replace what is currently configured so 
                // we need to add all currently configured ips to the request..
                var blackList = await SendGetIpBlacklistRequest();

                if (blackList != null && blackList.ResponseCode == ResponseCode.Success)
                {
                    foreach (string blackListedIp in blackList.BlockedIpAddresses)
                    {
                        ip += $",{blackListedIp}";
                    }
                }
            }

            var result = await PerformaAPIRequest(new BlacklistIPRequest(_config, ip));
            var response = JsonConvert.DeserializeObject<SiteStatusResponse>(result);
            return new IPBlacklist(response);
        }

        private async Task<IPBlacklist> SendGetIpBlacklistRequest()
        {
            var result = await PerformaAPIRequest(new SiteStatusRequest(_config));
            var response = JsonConvert.DeserializeObject<SiteStatusResponse>(result);
            return new IPBlacklist(response);
        }

        private async Task<ResponseCode> SendSiteCachePurgeRequest()
        {
            var result = await PerformaAPIRequest(new PurgeSiteCacheRequest(_config));
            var response = JsonConvert.DeserializeObject<ResponseBase>(result);
            return (ResponseCode)response.res;
        }

        private async Task<string> PerformaAPIRequest(RequestBase request)
        {
            try
            {
                var content = new FormUrlEncodedContent(request.RequestValues);
                var response = await _serviceClient.PostAsync(request.RequestPath, content);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                return ex.Message;
            }
        }

        #endregion
    }
}
