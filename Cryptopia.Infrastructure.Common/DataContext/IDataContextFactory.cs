namespace Cryptopia.Infrastructure.Common.DataContext
{
	public interface IDataContextFactory
	{
		IDataContext CreateContext();
		IDataContext CreateReadOnlyContext();
	}
}