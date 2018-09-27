using Cryptopia.API.Logging;
using Cryptopia.API.Logging.Base;
using System;
using System.Configuration;
using System.ServiceProcess;

namespace Cryptopia.OutboundService
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
            using (var processor = new OutboundService())
            {
                Console.WriteLine("Starting Wallet Outbound Service ...");
                processor.StartService();
                Console.WriteLine("Press Enter to terminate ...");
                Console.ReadLine();
                processor.StopService();
            }
#else

            LoggingManager.AddLog(new FileLogger(location, "OutboundService", level));
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new OutboundService() 
            };
            ServiceBase.Run(ServicesToRun);
            LoggingManager.Destroy();
#endif
        }
    }
}
