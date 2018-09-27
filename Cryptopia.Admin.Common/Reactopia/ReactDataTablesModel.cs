namespace Cryptopia.Admin.Common.Reactopia
{
	using System.Collections.Generic;
	using Cryptopia.Infrastructure.Common.DataTables;

	public class ReactDataTablesModel : IDataTablesModel
	{
		public ReactDataTablesModel()
		{
			sColumnNames = new List<string>();
			bSortable = new List<bool>();
			bSearchable = new List<bool>();
			sSearchValues = new List<string>();
			iSortCol = new List<int>();
			sSortDir = new List<string>();
			bEscapeRegexColumns = new List<bool>();
		}

		public bool bEscapeRegex { get; set; }
		public List<bool> bEscapeRegexColumns { get; set; }
		public List<bool> bSearchable { get; set; }
		public List<bool> bSortable { get; set; }
		public int iColumns { get; set; }
		public int iDisplayLength { get; set; }
		public int iDisplayStart { get; set; }
		public List<int> iSortCol { get; set; }
		public int iSortingCols { get; set; }
		public List<string> sColumnNames { get; set; }
		public int sEcho { get; set; }
		public string sSearch { get; set; }
		public List<string> sSearchValues { get; set; }
		public List<string> sSortDir { get; set; }

		public DataTablesModel Map()
		{
			return new DataTablesModel
			{
				bEscapeRegex = bEscapeRegex,
				bEscapeRegexColumns = bEscapeRegexColumns,
				bSearchable = bSearchable,
				bSortable = bSortable,
				iColumns = iColumns,
				iDisplayLength = iDisplayLength,
				iDisplayStart = iDisplayStart,
				iSortCol = iSortCol,
				iSortingCols = iSortingCols,
				sColumnNames = sColumnNames,
				sEcho = sEcho,
				sSearch = sSearch,
				sSearchValues = sSearchValues,
				sSortDir = sSortDir
			};
		}

	}
}