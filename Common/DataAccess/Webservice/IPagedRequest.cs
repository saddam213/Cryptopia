namespace Cryptopia.Common.Webservice
{
    public interface IPagedRequest : IRequest
    {
        int Page { get; set; }

        int ItemsPerPage { get; set; }

        string SearchTerm { get; set; }

        int ColumnOrderNumber { get; set; }

        string ColumnOrderDirection { get; set; }
    }
}
