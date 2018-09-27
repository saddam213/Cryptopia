using System.Collections.Generic;

namespace Cryptopia.Infrastructure.Common.DataTables
{
	public class DataTablesModel : IDataTablesModel
	{
		public int iDisplayStart { get; set; }
		public int iDisplayLength { get; set; }
		public int iColumns { get; set; }
		public string sSearch { get; set; }
		public bool bEscapeRegex { get; set; }
		public int iSortingCols { get; set; }
		public int sEcho { get; set; }
		public List<string> sColumnNames { get; set; }
		public List<bool> bSortable { get; set; }
		public List<bool> bSearchable { get; set; }
		public List<string> sSearchValues { get; set; }
		public List<int> iSortCol { get; set; }
		public List<string> sSortDir { get; set; }
		public List<bool> bEscapeRegexColumns { get; set; }

		public DataTablesModel()
		{
			sColumnNames = new List<string>();
			bSortable = new List<bool>();
			bSearchable = new List<bool>();
			sSearchValues = new List<string>();
			iSortCol = new List<int>();
			sSortDir = new List<string>();
			bEscapeRegexColumns = new List<bool>();
		}

		public DataTablesModel(int iColumns)
		{
			this.iColumns = iColumns;
			sColumnNames = new List<string>(iColumns);
			bSortable = new List<bool>(iColumns);
			bSearchable = new List<bool>(iColumns);
			sSearchValues = new List<string>(iColumns);
			iSortCol = new List<int>(iColumns);
			sSortDir = new List<string>(iColumns);
			bEscapeRegexColumns = new List<bool>(iColumns);
		}
	}
}
