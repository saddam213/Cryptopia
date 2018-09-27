using Cryptopia.Common.DataContext;

namespace Cryptopia.Data.DataContext
{
    public class MonitoringDataContextFactory : IMonitoringDataContextFactory
    {
        public IMonitoringDataContext CreateContext()
        {
            return new MonitoringDataContext();
        }

        public IMonitoringDataContext CreateReadOnlyContext()
        {
            var context = new MonitoringDataContext();
            context.Configuration.AutoDetectChangesEnabled = false;
            context.Configuration.LazyLoadingEnabled = false;
            context.Configuration.ProxyCreationEnabled = false;
            return context;
        }
    }
}
