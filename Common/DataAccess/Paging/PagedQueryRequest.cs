namespace Cryptopia.Common.DataAccess
{
	public class PagedQueryRequest
    {
        public PagedQueryRequest() { }
        public PagedQueryRequest(int pageNum, int itemsPerPage, int columnSortNum = 0, bool columSortAcending = true,string search = "")
        {        
            Page = pageNum;
            ItemsPerPage = itemsPerPage;
            SearchTerm = search;
            ColumnOrderNumber = columnSortNum;
            ColumnOrderAcending = columSortAcending;
        }
        public int Page { get; set; }

        public int ItemsPerPage { get; set; }

        public string SearchTerm { get; set; }

        public int ColumnOrderNumber { get; set; }

        public bool ColumnOrderAcending { get; set; }
    }
}
