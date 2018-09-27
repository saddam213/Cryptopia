using System.Configuration;
using Cryptopia.API.Logging;
using Cryptopia.API.Logging.Base;
using System;
using System.ServiceProcess;
using System.IO;

namespace Cryptopia.DepositTrackerService
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
			using (var processor = new DepositTrackerService())
			{
				processor.StartService();
				Console.WriteLine("Press Enter to terminate ...");
				Console.ReadLine();
				processor.StopService();
			}
#else
			var walletHostName = ConfigurationManager.AppSettings["WalletHostName"];
			LoggingManager.AddLog(new FileLogger(Path.Combine(location, walletHostName), "DepositTracker", level));
			ServiceBase[] ServicesToRun;
			ServicesToRun = new ServiceBase[]
			{
								new DepositTrackerService()
			};
			ServiceBase.Run(ServicesToRun);
			LoggingManager.Destroy();
#endif
		}


	}
}
