using Cryptopia.Base.Logging;
using Cryptopia.PoolService.Implementation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cryptopia.PoolService
{
	public partial class PoolService : ServiceBase
	{
		private readonly Log Log = LoggingManager.GetLog(typeof(PoolService));
		private ServiceHost _serviceHost;
		private PoolTracker _poolProcessor;
		private CancellationTokenSource _cancellationTokenSource;

		public PoolService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			StartService();
		}

		protected override void OnStop()
		{
			StopService();
		}

		public void StartService()
		{
			Log.Message(LogLevel.Info, "[OnStart] - Starting pool service...");

			Log.Message(LogLevel.Info, "[OnStart] - Opening service host...");
			_serviceHost = new ServiceHost(typeof(PoolDataService));
			_serviceHost.Open();

			Log.Message(LogLevel.Info, "[OnStart] - Starting pool tracker...");
			_cancellationTokenSource = new CancellationTokenSource();
			_poolProcessor = new PoolTracker(_cancellationTokenSource.Token);
			_poolProcessor.Start();

			Log.Message(LogLevel.Info, "[OnStart] - Pool service started.");
		}

		public void StopService()
		{
			Log.Message(LogLevel.Info, "[OnStop] - Stopping Pool service...");

			Log.Message(LogLevel.Info, "[OnStop] - Stopping pool tracker.");
			_cancellationTokenSource.Cancel();
			_poolProcessor.Stop();

			while (_poolProcessor.Running)
			{
				Task.Delay(5000).Wait();
				RequestAdditionalTime(5000);
			}
			Log.Message(LogLevel.Info, "[OnStop] - Pool tracker stopped.");


			if (_serviceHost != null)
			{
				Log.Message(LogLevel.Info, "[OnStop] - Stopping service host.");
				_serviceHost.Close();
				_serviceHost = null;
				Log.Message(LogLevel.Info, "[OnStop] - Service host stopped.");
			}


			Log.Message(LogLevel.Info, "[OnStop] - Pool service stopped.");
		}
	}
}
