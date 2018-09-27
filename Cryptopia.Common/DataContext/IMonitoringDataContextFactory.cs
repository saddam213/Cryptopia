namespace Cryptopia.Common.DataContext
{
    public interface IMonitoringDataContextFactory
    {
        IMonitoringDataContext CreateContext();
        IMonitoringDataContext CreateReadOnlyContext();
    }
}
