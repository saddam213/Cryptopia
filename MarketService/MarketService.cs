using Cryptopia.API.Logging.Base;
using Cryptopia.Logging;
using Cryptopia.PoolService.Implementation;
using System;
using System.Configuration;
using System.ServiceProcess;

namespace MarketService
{
	public partial class MarketService : ServiceBase
    {
        private readonly Log Log = LoggingManager.GetLog(typeof(MarketService));
        private MarketMonitor _marketMonitor;

        public MarketService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Log.Message(LogLevel.Info, "[OnStart] - Starting pool service...");

            int pollPeriod;
            if (!int.TryParse(ConfigurationManager.AppSettings["PollPeriod"], out pollPeriod))
            {
                Log.Message(LogLevel.Error, "[OnStart] - Poll period '{0}' is invalid", pollPeriod);
                throw new ApplicationException("Invalid PollPeriod set");
            }

            string emailUser = ConfigurationManager.AppSettings["SMTP_User"];
            if (string.IsNullOrEmpty(emailUser))
            {
                Log.Message(LogLevel.Error, "[OnStart] - EmailUser '{0}' is invalid", emailUser);
                throw new ApplicationException("Invalid EmailUser set");
            }

            string emailPass = ConfigurationManager.AppSettings["SMTP_Password"];
            if (string.IsNullOrEmpty(emailPass))
            {
                Log.Message(LogLevel.Error, "[OnStart] - EmailPass '{0}' is invalid", emailPass);
                throw new ApplicationException("Invalid EmailPass set");
            }

            string emailServer = ConfigurationManager.AppSettings["SMTP_Server"];
            if (string.IsNullOrEmpty(emailServer))
            {
                Log.Message(LogLevel.Error, "[OnStart] - EmailServer '{0}' is invalid", emailServer);
                throw new ApplicationException("Invalid EmailServer set");
            }

            int emailPort;
            if (!int.TryParse(ConfigurationManager.AppSettings["SMTP_Port"], out emailPort))
            {
                Log.Message(LogLevel.Error, "[OnStart] - EmailPort '{0}' is invalid", emailPort);
                throw new ApplicationException("Invalid EmailPort set");
            }

            try
            {
                // start polling marketitems
                Log.Message(LogLevel.Info, "[OnStart] - Starting MarketMonitor.");
                _marketMonitor = new MarketMonitor(pollPeriod, emailUser, emailPass, emailServer, emailPort);
                _marketMonitor.Start();
                Log.Message(LogLevel.Info, "[OnStart] - Market service started.");
            }
            catch (Exception ex)
            {
                Log.Exception("[OnStart] - An exception occured starting service", ex);
                throw;
            }

            Log.Message(LogLevel.Info, "[OnStart] - Service started.");
        }

        protected override void OnStop()
        {
            Log.Message(LogLevel.Info, "[OnStop] - Stopping service.");

            _marketMonitor.Stop();

            Log.Message(LogLevel.Info, "[OnStop] - Service stopped.");
        }

        /// <summary>
        /// Starts the service.
        /// </summary>
        public void StartService()
        {
            OnStart(null);
        }

        /// <summary>
        /// Stops the service.
        /// </summary>
        public void StopService()
        {
            OnStop();
        }
    }
}
