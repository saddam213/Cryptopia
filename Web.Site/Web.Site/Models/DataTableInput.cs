namespace Web.Site.Models
{
	public class DataTableInput
    {
        /// <summary>
        /// Gets or sets the echo.
        /// </summary>
        public string sEcho { get; set; }

        /// <summary>
        /// Gets or sets the serach string.
        /// </summary>
        public string sSearch { get; set; }

        /// <summary>
        /// Gets or sets the amount of items to display in the table.
        /// </summary>
        public int iDisplayLength { get; set; }

        /// <summary>
        /// Gets or sets the index to start takingthe items from the list.
        /// </summary>
        public int iDisplayStart { get; set; }

        public int iSortCol_0 { get; set; }

        public string sSortDir_0 { get; set; }

        /// <summary>
        /// Gets the page number requested.
        /// </summary>
        public int PageNumber
        {
            get { return iDisplayLength > 0 ? (iDisplayStart / iDisplayLength) + 1 : 1; }
        }
    } 
}