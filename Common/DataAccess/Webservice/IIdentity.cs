using System;

namespace Cryptopia.Common.Webservice
{
    public interface IIdentity
    {
    }

    public interface IIdentity<T> : IIdentity where T : IComparable
    {
        T Id { get; set; }
    }
}
