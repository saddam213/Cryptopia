namespace Cryptopia.Common.DataContext
{
	public interface IExchangeDataContextFactory
	{
		IExchangeDataContext CreateContext();
		IExchangeDataContext CreateReadOnlyContext();
	}
}