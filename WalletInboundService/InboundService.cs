using System;
using System.ServiceModel;
using System.ServiceProcess;
using Cryptopia.API.Logging.Base;
using Cryptopia.Logging;

namespace Cryptopia.InboundService
{
	/// <summary>
	/// Service for communicating with wallets for deposit related tasks
	/// </summary>
	public partial class InboundService : ServiceBase
	{
		#region Fields

		private ServiceHost _serviceHost;
		private readonly Log Log = LoggingManager.GetLog(typeof(InboundService));

		#endregion

		#region Constructor

		/// <summary>
		/// Initializes a new instance of the <see cref="InboundService"/> class.
		/// </summary>
		public InboundService()
		{
			InitializeComponent();
		}

		#endregion

		#region Properties

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
			try
			{
				Log.Message(LogLevel.Info, "[OnStart] - Starting inbound service...");
				// open service for incomming calls
				Log.Message(LogLevel.Info, "[OnStart] - Opening service host");
				_serviceHost = new ServiceHost(typeof(WalletInboundService));
				_serviceHost.Open();
				Log.Message(LogLevel.Info, "[OnStart] - Inbound service started.");
			}
			catch (Exception ex)
			{
				Log.Exception("[OnStart] - An exception occured starting service", ex);
				throw;
			}
		}

		protected override void OnStop()
		{
			Log.Message(LogLevel.Info, "[OnStop] - Stopping inbound service.");
			if (_serviceHost != null)
			{
				Log.Message(LogLevel.Info, "[OnStop] - Closing service host.");
				_serviceHost.Close();
				_serviceHost = null;
			}
			Log.Message(LogLevel.Info, "[OnStop] - Wallet inbound stopped.");
		}

		public void StartService()
		{
			OnStart(null);
		}

		public void StopService()
		{
			OnStop();
		}

		#endregion
	}
}
