using System;

namespace Cryptopia.Infrastructure.Incapsula.Common.Classes.Stats
{
    public interface IDataPoint
    {
        DateTime Day { get; }
        int Number { get; }
        string DateString { get; }
    }
}
