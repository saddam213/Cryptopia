namespace SpriteBuilderService
{
	using System;
	using System.ServiceModel;
	using System.ServiceProcess;

	public partial class ServiceCore : ServiceBase
	{
		#region Constructor

		public ServiceCore()
		{
			InitializeComponent();
		}

		#endregion

		#region Fields

		private ServiceHost _serviceHost;
		//private readonly Log _log = LoggingManager.GetLog(typeof(ServiceCore));

		#endregion

		#region Overrides

		/// <inheritdoc />
		/// <summary>
		///     When implemented in a derived class, executes when a Start command is sent to the service by the Service Control
		///     Manager (SCM) or when the operating system starts (for a service that starts automatically). Specifies actions to
		///     take when the service starts.
		/// </summary>
		/// <param name="args">Data passed by the start command.</param>
		/// <exception cref="T:System.ApplicationException">
		///     DataFile does not exist
		///     or
		///     Invalid PollPeriod set
		/// </exception>
		protected override void OnStart(string[] args)
		{
			try
			{
				//_log.Message(LogLevel.Info, $"[OnStart] - Starting {nameof(AdmintopiaService)} service...");
				// open service for incomming calls
				//_log.Message(LogLevel.Info, "[OnStart] - Opening service host");
				_serviceHost = new ServiceHost(typeof(SpriteBuilderService));
				_serviceHost.Open();
				//_log.Message(LogLevel.Info, $"[OnStart] - {nameof(AdmintopiaService)} started.");
			}
			catch (Exception)
			{
				//_log.Exception("[OnStart] - An exception occured starting service", ex);
				throw;
			}
		}

		protected override void OnStop()
		{
			//_log.Message(LogLevel.Info, $"[OnStop] - Stopping {nameof(AdmintopiaService)}.");
			if (_serviceHost != null)
			{
				//_log.Message(LogLevel.Info, "[OnStop] - Closing service host.");
				_serviceHost.Close();
				_serviceHost = null;
			}
			//_log.Message(LogLevel.Info, $"[OnStop] - {nameof(AdmintopiaService)} stopped.");
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