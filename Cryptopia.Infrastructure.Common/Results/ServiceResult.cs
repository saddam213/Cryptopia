namespace Cryptopia.Infrastructure.Common.Results
{
	public class ServiceResult : IServiceResult
	{
		public ServiceResult(bool success)
		{
			Success = success;
		}
		public ServiceResult(bool success, string message, params object[] formatParams)
		{
			Success = success;
			Message = string.Format(message, formatParams);
		}
		public bool Success { get; set; }
		public string Message { get; set; }
	}

	public class ServiceResult<T> : IServiceResult<T>
	{
		public ServiceResult() { }
		public ServiceResult(bool success)
		{
			Success = success;
		}
		public ServiceResult(bool success, T result)
		{
			Result = result;
			Success = success;
		}
		public ServiceResult(bool success, string message, params object[] messageParams)
		{
			Success = success;
			Message = string.Format(message, messageParams);
		}

		public ServiceResult(bool success, T result, string message, params object[] messageParams)
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
