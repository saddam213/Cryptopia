namespace Cryptopia.Infrastructure.Common.Results
{
	public class WriterResult : IWriterResult
	{
		public WriterResult() { }
		public WriterResult(bool success)
		{
			Success = success;
		}
		public WriterResult(bool success, string message, params object[] messageParams)
		{
			Success = success;
			Message = string.Format(message, messageParams);
		}

		public bool Success { get; set; }
		public string Message { get; set; }

	}

	public class WriterResult<T> : IWriterResult<T>
	{
		public WriterResult() { }
		public WriterResult(bool success)
		{
			Success = success;
		}
		public WriterResult(bool success, T result)
		{
			Result = result;
			Success = success;
		}
		public WriterResult(bool success, string message, params object[] messageParams)
		{
			Success = success;
			Message = string.Format(message, messageParams);
		}

		public WriterResult(bool success, T result, string message, params object[] messageParams)
		{
			Result = result;
			Success = success;
			Message = string.Format(message, messageParams);
		}

		public T Result { get; set; }
		public bool Success { get; set; }
		public string Message { get; set; }
		
		public bool HasResult
		{
			get { return Result != null; }
		}
	}
}
