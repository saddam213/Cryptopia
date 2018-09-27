using Cryptopia.Base.Logging;
using Cryptopia.LottoService.Implementation;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace Cryptopia.LottoService
{
	public partial class LottoProcessorService : ServiceBase
	{
		private readonly Log Log = LoggingManager.GetLog(typeof(LottoProcessorService));
		private LottoProcessor _lottoProcessor;
		private CancellationTokenSource _cancellationTokenSource;

		public LottoProcessorService()
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
			_cancellationTokenSource = new CancellationTokenSource();
			_lottoProcessor = new LottoProcessor(_cancellationTokenSource.Token);
			_lottoProcessor.Start();

			Log.Message(LogLevel.Info, "[OnStart] - Lotto service started.");
		}

		public void StopService()
		{
			Log.Message(LogLevel.Info, "[OnStop] - Stopping Lotto service...");

			_cancellationTokenSource.Cancel();
			_lottoProcessor.Stop();

			while (_lottoProcessor.Running)
			{
				Task.Delay(5000).Wait();
				RequestAdditionalTime(5000);
			}

			Log.Message(LogLevel.Info, "[OnStop] - Lotto service stopped.");
		}
	}
}
