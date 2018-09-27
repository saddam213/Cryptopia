using Cryptopia.Base.Logging;
using Cryptopia.IntegrationService.Implementation;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace Cryptopia.IntegrationService
{
	public partial class IntegrationService : ServiceBase
	{
		private readonly Log Log = LoggingManager.GetLog(typeof(IntegrationService));
		private IProcessor _paytopiaProcessor;
		private IProcessor _exchangeProcessor;
		private IProcessor _referralProcessor;
        private IProcessor _incapsulaProcessor;
		private IProcessor _nzdtProcessor;
        private IProcessor _cefsProcessor;
        private CancellationTokenSource _cancellationTokenSource;

		public IntegrationService()
		{
			InitializeComponent();
		}

		protected override void OnStart(string[] args)
		{
			Log.Message(LogLevel.Info, "[OnStart] - Starting service.");
			_cancellationTokenSource = new CancellationTokenSource();
			_exchangeProcessor = new ExchangeProcessor(_cancellationTokenSource.Token);
			_paytopiaProcessor = new PaytopiaProcessor(_cancellationTokenSource.Token);
			_nzdtProcessor = new NzdtProcessor(_cancellationTokenSource.Token);
			_cefsProcessor = new CEFSProcessor(_cancellationTokenSource.Token);
			_referralProcessor = new ReferralProcessor(_cancellationTokenSource.Token);
			_incapsulaProcessor = new IncapsulaProcessor(_cancellationTokenSource.Token);

			_exchangeProcessor.Start();
			_paytopiaProcessor.Start();
			_nzdtProcessor.Start();
			_cefsProcessor.Start();
			_referralProcessor.Start();
			_incapsulaProcessor.Start();

			Log.Message(LogLevel.Info, "[OnStart] - Service started.");
		}

		protected override void OnStop()
		{
			Log.Message(LogLevel.Info, "[OnStop] - Stopping service.");
			_cancellationTokenSource.Cancel();
			_exchangeProcessor.Stop();
			_paytopiaProcessor.Stop();
			_nzdtProcessor.Stop();
			_cefsProcessor.Stop();
			_referralProcessor.Stop();
			_incapsulaProcessor.Stop();


			while (_exchangeProcessor.Running
				|| _paytopiaProcessor.Running
				|| _referralProcessor.Running
				|| _nzdtProcessor.Running
				|| _cefsProcessor.Running
				|| _incapsulaProcessor.Running)
			{
				Task.Delay(5000).Wait();
				RequestAdditionalTime(5000);
			}

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
