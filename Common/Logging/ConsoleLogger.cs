using System;
using Cryptopia.API.Logging.Base;
using Cryptopia.Logging;

namespace Cryptopia.API.Logging
{
    /// <summary>
    /// Outputs messages to the console
    /// </summary>
    public class ConsoleLogger : Logger
    {
        public ConsoleLogger(LogLevel level)
            : base(level)
        {
        }

        protected override void LogQueuedMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}