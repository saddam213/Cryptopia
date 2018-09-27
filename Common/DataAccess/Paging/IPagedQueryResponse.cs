using System.Collections.Generic;

namespace Cryptopia.Common.DataAccess
{
	public interface IPagedQueryResponse<T>
    {
        int ItemsPerPage { get; set; }
        int TotalItems { get; set; }
        int PageCount { get; }
        IEnumerable<T> PageResults { get; set; }
    }
}
