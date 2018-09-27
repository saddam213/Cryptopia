using System.Collections.Generic;

namespace Cryptopia.Common.Webservice
{
    public interface IPagedResponse<T> : IResponse
    {
        int Page { get; set; }

        List<T> PageResults { get; set; }

        int TotalPages { get; set; }

        int TotalCount { get; set; }
    }
}
