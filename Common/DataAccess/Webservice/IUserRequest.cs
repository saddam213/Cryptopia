using System;

namespace Cryptopia.Common.Webservice
{
    public interface IUserRequest : IRequest
    {
        Guid UserId { get; set; }
    }
}
