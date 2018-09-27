using Cryptopia.Base.Logging;
using System;
using System.Configuration;
using System.ServiceProcess;

namespace Cryptopia.LottoService
{
	static class Program
	{
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		static void Main()
		{
			var level = LoggingManager.LogLevelFromString(ConfigurationManager.AppSettings["LogLevel"]);
			var location = ConfigurationManager.AppSettings["LogLocation"];

#if DEBUG
			LoggingManager.AddLog(new ConsoleLogger(level));
			using (var processor = new LottoProcessorService())
			{
				Console.WriteLine("Starting Lotto Service ...");
				processor.StartService();
				Console.WriteLine("Press Enter to terminate ...");
				Console.ReadLine();
				processor.StopService();
			}
#else
			LoggingManager.AddLog(new FileLogger(location, "LottoService", level));
			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[] 
			{ 
				new LottoProcessorService() 
			};
			ServiceBase.Run(ServicesToRun);
			LoggingManager.Destroy();
#endif
		}
	}
}
