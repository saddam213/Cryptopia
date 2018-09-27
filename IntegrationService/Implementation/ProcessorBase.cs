
using Cryptopia.Base.Logging;

namespace Cryptopia.IntegrationService.Implementation
{
    public abstract class ProcessorBase<T> : IProcessor
    {
        #region Protected Members

        protected abstract Log Log { get; }
        protected bool _isRunning;
        protected bool _isEnabled;

        #endregion

        public ProcessorBase(T parameters)
        { }

        #region IProcessor Members

        public bool Running { get { return _isRunning; } }

        public void Start()
        {
            if (_isEnabled)
                return;

            Log.Message(LogLevel.Info, StartLog);
            _isRunning = true;
            _isEnabled = true;
            Process();
        }

        public void Stop()
        {
            Log.Message(LogLevel.Info, StopLog);
            _isEnabled = false;
        }

        #endregion

        #region Abstract Members

        public abstract string StartLog { get; }

        public abstract string StopLog { get; }

        protected abstract void Process();

        #endregion
    }
}
