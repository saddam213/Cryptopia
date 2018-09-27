using Cryptopia.Base.Logging;
using Cryptopia.RewardService.Implementation;
using System;
using System.Configuration;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace Cryptopia.RewardService
{
	public partial class RewardService : ServiceBase
	{
		private readonly Log Log = LoggingManager.GetLog(typeof(RewardService));
		private RewardProcessor _rewardProcessor;
		private CancellationTokenSource _cancellationTokenSource;

		public RewardService()
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
			Log.Message(LogLevel.Info, "[OnStart] - Starting reward service...");

			int pollPeriod = 1;
			if (!int.TryParse(ConfigurationManager.AppSettings["PollPeriod"], out pollPeriod))
			{
				Log.Message(LogLevel.Error, "[OnStart] - PollPeriod '{0}' is invalid", pollPeriod);
				throw new ApplicationException("Invalid PollPeriod set");
			}

			_cancellationTokenSource = new CancellationTokenSource();
			_rewardProcessor = new RewardProcessor(pollPeriod, _cancellationTokenSource.Token);
			_rewardProcessor.Start();

			Log.Message(LogLevel.Info, "[OnStart] - Reward service started.");
		}

		public void StopService()
		{
			Log.Message(LogLevel.Info, "[OnStop] - Stopping Reward service...");

			_cancellationTokenSource.Cancel();
			_rewardProcessor.Stop();

			while (_rewardProcessor.Running)
			{
				Task.Delay(5000).Wait();
				RequestAdditionalTime(5000);
			}

			Log.Message(LogLevel.Info, "[OnStop] - Reward service stopped.");
		}
	}
}
