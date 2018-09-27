namespace Cryptopia.Base.Logging
{
	/// <summary>
	/// Outputs messages to the console
	/// </summary>
	public class DBLogger : Logger
	{
		private readonly string _connectionString;
		public DBLogger(LogLevel level, string connectionString)
				: base(level)
		{
			_connectionString = connectionString;

		}

		protected override void LogQueuedMessage(string message)
		{
			//var repo = new Repository<Cryptopia.API.Objects.Tables.Log>(_connectionString);
			//var log = repo.CreateInstance();
			//log.Data = message;
			//repo.Insert(log);
			//repo.Save();

		}
	}



}