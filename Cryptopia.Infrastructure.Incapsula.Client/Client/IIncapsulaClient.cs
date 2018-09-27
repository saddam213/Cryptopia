
using System;
using System.Threading.Tasks;

using Cryptopia.Infrastructure.Incapsula.Common.Classes;
using Cryptopia.Infrastructure.Incapsula.Common.Enums;
using Cryptopia.Infrastructure.Incapsula.Common.AppConfiguration;

namespace Cryptopia.Infrastructure.Incapsula.Client
{
  /// <summary>
  /// Interface for accessing the Incapsula Client API.
  /// </summary>
  public interface IIncapsulaClient : IDisposable
  {
    Task<IApplicationConfiguration> GetClientConfiguration();

    /// <summary>
    /// Use this operation to get the status of a site.
    /// </summary>
    /// <returns>An object representation of recent site status in the form of <seealso cref="SiteStatus"/></returns>
    Task<SiteStatus> GetSiteStatus();

    /// <summary>
    /// Use this operation to get a report for a site.
    /// </summary>
    /// <param name="range">The range of time to the get a reportfor. <seealso cref="TimeRange"/></param>
    /// <returns>An object representation of recent site report in the form of <seealso cref="SiteReport"/></returns>
    Task<SiteReport> GetSiteReport(TimeRange range);

    /// <summary>
    /// Use this operation to get a report for a site for a specific date and time range.
    /// </summary>
    /// <param name="rangeStart">The start of the report period.</param>
    /// <param name="rangeEnd">The end of the report period.</param>
    /// <returns>An object representation of recent site report in the form of <seealso cref="SiteReport"/</returns>
    Task<SiteReport> GetSiteReport(DateTime rangeStart, DateTime rangeEnd);

    /// <summary>
    /// Use this operation to get site statistics for one or more sites. 
    /// This operation may return multiple statistics, as specified in the stats parameter.
    /// </summary>
    /// <returns>An object representation of recent site statistics in the form of <seealso cref="SiteStatistics"/></returns>
    Task<SiteStatistics> GetSiteStats(StatisticsValue stats, TimeRange range);

    /// <summary>
    /// Use this operation to get site statistics for one or more sites. 
    /// This operation may return multiple statistics, as specified in the stats parameter.
    /// </summary>
    /// <param name="rangeStart">The start of the report period.</param>
    /// <param name="rangeEnd">The end of the report period.</param>
    /// <returns>An object representation of recent site statistics in the form of <seealso cref="SiteStatistics"/></returns>
    Task<SiteStatistics> GetSiteStats(StatisticsValue stats, DateTime rangeStart, DateTime rangeEnd);

    /// <summary>
    /// Use this operation to get a log of recent visits to a website.
    /// The visits are fetched in reverse chronological order, starting with the most recent visit. 
    /// Not all visits are recorded - only visits with abnormal activity are recorded e.g. violation of security rules, visits from black-listed IPs/Countries, etc. 
    /// A visit may still be updated even after it was retrieved. Visits are aggregated into a session, and Incapsula may use a suppression mechanism to trim repetitive events. 
    /// This session is set by the Incapsula reverse proxy and does not correlate with the application session set between the end user browser and the origin server. 
    /// To retrieve only visits that will no longer be updated, use the list_live_visits parameter.
    /// </summary>
    /// <returns>An object representation of recent site visits in the form of <seealso cref="SiteVisits"/></returns>
    Task<SiteVisits> GetSiteVisits(TimeRange range);

    /// <summary>
    /// As per GetSiteVisits but for a specific time period.
    /// </summary>
    /// <param name="rangeStart">The start of the report period.</param>
    /// <param name="rangeEnd">The end of the report period.</param>
    /// <returns></returns>
    Task<SiteVisits> GetSiteVisits(DateTime rangeStart, DateTime rangeEnd);

    /// <summary>
    /// Adds the specified IP address to the blacklist for the site configured in the app.config.
    /// </summary>
    /// <param name="ip">The IP address to blacklist.</param>
    /// <returns>The outcome of the request and a list of currently blocked IP Addresses. <seealso cref="IPBlacklist"/></returns>
    Task<IPBlacklist> BlacklistIp(string ip);
    
    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    Task<IPBlacklist> GetIpBlacklist();

    /// <summary>
    /// Use this operation to purge all cached content on Incapsula proxy servers for the configured site. 
    /// Incapsula Proxy servers keep cached content of the site in order to accelerate page load times for your users. 
    /// When you want this cached content to be refreshed (for example, after making adjustments in your site) you can use this API call. 
    /// </summary>
    /// <returns>The result of the request</returns>
    Task<ResponseCode> PurgeSiteCache();
  }
}
