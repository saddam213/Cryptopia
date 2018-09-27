using Cryptopia.API.Logging.Base;
using Cryptopia.Logging;
using Cryptopia.OutboundService.Implementation;
using System;
using System.Configuration;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace Cryptopia.OutboundService
{
	/// <summary>
	/// Service for polling the database for new withdraws and updating withdraw transaction information
	/// </summary>
	public partial class OutboundService : ServiceBase
    {
        /// <summary>
        /// The withdraw tracker
        /// </summary>
        private WithdrawTracker _withdrawTracker;

        /// <summary>
        /// The cancellation token source
        /// </summary>
        private CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// The log instance
        /// </summary>
        private readonly Log Log = LoggingManager.GetLog(typeof(OutboundService));
       

        /// <summary>
        /// Initializes a new instance of the <see cref="OutboundService"/> class.
        /// </summary>
        public OutboundService()
        {
            InitializeComponent();
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Start command is sent to the service by the Service Control Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to take when the service starts.
        /// </summary>
        /// <param name="args">Data passed by the start command.</param>
        protected override void OnStart(string[] args)
        {
            int pollWithdrawPeriod;
            if (!int.TryParse(ConfigurationManager.AppSettings["PollPeriod"], out pollWithdrawPeriod))
            {
                Log.Message(LogLevel.Error, "[OnStart] - PollPeriod period '{0}' is invalid", pollWithdrawPeriod);
                throw new ApplicationException("Invalid PollPeriod period set");
            }

			int pollWithdrawConfirmationPeriod;
			if (!int.TryParse(ConfigurationManager.AppSettings["PollConfirmationPeriod"], out pollWithdrawConfirmationPeriod))
			{
				Log.Message(LogLevel.Error, "[OnStart] - PollConfirmationPeriod period '{0}' is invalid", pollWithdrawConfirmationPeriod);
				throw new ApplicationException("Invalid PollConfirmationPeriod period set");
			}

            int maxConfirmations;
            if (!int.TryParse(ConfigurationManager.AppSettings["MaxConfirmationCount"], out maxConfirmations))
            {
                Log.Message(LogLevel.Error, "[OnStart] - MaxConfirmations '{0}' is invalid", maxConfirmations);
                throw new ApplicationException("Invalid MaxConfirmations set");
            }

            try
            {
                // start polling wallets
                Log.Message(LogLevel.Info, "[OnStart] - Starting withdraw tracker...");
                _cancellationTokenSource = new CancellationTokenSource();
                var cancelToken = _cancellationTokenSource.Token;
                _withdrawTracker = new WithdrawTracker(cancelToken, pollWithdrawPeriod,pollWithdrawConfirmationPeriod, maxConfirmations);
                Log.Message(LogLevel.Info, "[OnStart] - Withdraw tracker started.");
            }
            catch (Exception ex)
            {
                Log.Exception("An exceptoon occured starting withdraw tracker.", ex);
                throw;
            }
        }

        /// <summary>
        /// When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service stops running.
        /// </summary>
        protected override void OnStop()
        {
            Log.Message(LogLevel.Info, "[OnStop] - Stopping service.");
            _cancellationTokenSource.Cancel();
            while (_withdrawTracker.Running)
            {
                Log.Message(LogLevel.Info, "[OnStop] - Withdraw tracker still running, waiting for completion...");
                Task.Delay(5000).Wait();
                RequestAdditionalTime(5000);
            }
            Log.Message(LogLevel.Info, "[OnStop] - Withdraw tracker stopped.");
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
