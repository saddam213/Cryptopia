#if DEBUG

#else
using System.ServiceProcess;
#endif

namespace SpriteBuilderService
{
	using System;
	using System.Configuration;
	using Cryptopia.Base.Logging;

	internal static class Program
	{
		/// <summary>
		///     The main entry point for the application.
		/// </summary>
		public static void Main()
		{
			var level = LoggingManager.LogLevelFromString(ConfigurationManager.AppSettings["LogLevel"]);
			var location = ConfigurationManager.AppSettings["LogLocation"];

#if DEBUG
			//LoggingManager.AddLog(new ConsoleLogger(level));
			using (var processor = new ServiceCore())
			{
				processor.StartService();
				Console.WriteLine("Press Enter to terminate ...");
				Console.ReadLine();
				processor.StopService();
			}
#else
			LoggingManager.AddLog(new FileLogger(location, nameof(SpriteBuilderService), level));
			var servicesToRun = new ServiceBase[]
			{
				new ServiceCore()
			};
			ServiceBase.Run(servicesToRun);
			LoggingManager.Destroy();
#endif
		}
	}
}