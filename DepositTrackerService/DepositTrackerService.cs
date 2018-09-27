using System;
using System.Configuration;
using System.ServiceProcess;
using Cryptopia.API.Logging.Base;
using Cryptopia.DepositTrackerService.Implementation;
using Cryptopia.Logging;
using System.Threading;
using System.Threading.Tasks;

namespace Cryptopia.DepositTrackerService
{
	/// <summary>
	/// Service for communicating with wallets for deposit related tasks
	/// </summary>
	public partial class DepositTrackerService : ServiceBase
	{
		#region Fields

		private CancellationTokenSource _cancellationTokenSource;
        private DepositTracker _depositTracker;
		private readonly Log Log = LoggingManager.GetLog(typeof(DepositTrackerService));

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="DepositTrackerService"/> class.
		/// </summary>
		public DepositTrackerService()
		{
			InitializeComponent();
		}

		#endregion

		#region Overrides

		/// <summary>
		/// When implemented in a derived class, executes when a Start command is sent to the service by the Service Control Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to take when the service starts.
		/// </summary>
		/// <param name="args">Data passed by the start command.</param>
		/// <exception cref="System.ApplicationException">
		/// DataFile does not exist
		/// or
		/// Invalid PollPeriod set
		/// </exception>
		protected override void OnStart(string[] args)
		{
            int pollPeriod;
            Log.Message(LogLevel.Info, "[OnStart] - Starting deposit service...");
			if (!int.TryParse(ConfigurationManager.AppSettings["PollPeriod"], out pollPeriod))
			{
				Log.Message(LogLevel.Error, "[OnStart] - Poll period '{0}' is invalid", pollPeriod);
				throw new ApplicationException("Invalid PollPeriod set");
			}

            string hostname = ConfigurationManager.AppSettings["WalletHost"];
            if (string.IsNullOrEmpty(hostname))
			{
				Log.Message(LogLevel.Error, "[OnStart] - WalletHost '{0}' is invalid", hostname);
				throw new ApplicationException("Invalid WalletHost set");
			}

			try
			{
				// start polling wallets
				Log.Message(LogLevel.Info, "[OnStart] - Starting Deposit Tracker...");
				_cancellationTokenSource = new CancellationTokenSource();
				var cancelToken = _cancellationTokenSource.Token;
				_depositTracker = new DepositTracker(cancelToken, pollPeriod, hostname);
                Log.Message(LogLevel.Info, "[OnStart] - Deposit Tracker started.");
			}
			catch (Exception ex)
			{
				Log.Exception("[OnStart] - An exception occured starting service", ex);
				throw;
			}
            Log.Message(LogLevel.Info, "[OnStart] - Inbound service started.");
        }

		/// <summary>
		/// When implemented in a derived class, executes when a Stop command is sent to the service by the Service Control Manager (SCM). Specifies actions to take when a service stops running.
		/// </summary>
		protected override void OnStop()
		{
			Log.Message(LogLevel.Info, "[OnStop] - Stopping service.");
			_cancellationTokenSource.Cancel();
			while (_depositTracker.Running)
			{
				Log.Message(LogLevel.Info, "[OnStop] - Deposit tracker still running, waiting for completion...");
				Task.Delay(5000).Wait();
				RequestAdditionalTime(5000);
			}
			Log.Message(LogLevel.Info, "[OnStop] - Deposit tracker stopped.");
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

		#endregion
	}
}
