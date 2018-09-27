namespace Cryptopia.Infrastructure.Common.Results
{
	public interface IServiceResult
	{
		bool Success { get; set; }
		string Message { get; set; }
	}

	public interface IServiceResult<T>
	{
		bool Success { get; set; }
		string Message { get; set; }
		T Result { get; set; }
		bool HasResult { get; }
	}
}
