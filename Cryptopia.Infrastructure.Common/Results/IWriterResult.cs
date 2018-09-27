namespace Cryptopia.Infrastructure.Common.Results
{
	public interface IWriterResult
	{
		bool Success { get; set; }
		string Message { get; set; }
	}

	public interface IWriterResult<T> : IWriterResult
	{
		T Result { get; set; }
		bool HasResult { get; }
	}
}