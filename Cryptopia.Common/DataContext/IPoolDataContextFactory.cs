namespace Cryptopia.Common.DataContext
{
	public interface IPoolDataContextFactory
	{
		IPoolDataContext CreateContext();
		IPoolDataContext CreateReadOnlyContext();
	}
}