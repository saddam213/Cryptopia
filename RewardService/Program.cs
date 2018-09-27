using Cryptopia.Base.Logging;
using System;
using System.Configuration;
using System.ServiceProcess;

namespace Cryptopia.RewardService
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
			using (var processor = new RewardService())
			{
				Console.WriteLine("Starting Reward Service ...");
				processor.StartService();
				Console.WriteLine("Press Enter to terminate ...");
				Console.ReadLine();
				processor.StopService();
			}
#else

            LoggingManager.AddLog(new FileLogger(location, "RewardService", level));
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new RewardService() 
            };
            ServiceBase.Run(ServicesToRun);
            LoggingManager.Destroy();
#endif
		}
	}
}
