using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
