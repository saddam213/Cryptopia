using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.DataAccess
{
    public class PagedQueryResponse<T> : IPagedQueryResponse<T>
    {
        public int ItemsPerPage { get; set; }

        public int TotalItems { get; set; }

        public int PageCount
        {
            get { return TotalItems / ItemsPerPage; }
        }

        public IEnumerable<T> PageResults { get; set; }
    }
}
