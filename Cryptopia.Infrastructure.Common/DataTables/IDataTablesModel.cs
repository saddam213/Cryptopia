using System.Collections.Generic;

namespace Cryptopia.Infrastructure.Common.DataTables
{
	public interface IDataTablesModel
	{
		bool bEscapeRegex { get; set; }
		List<bool> bEscapeRegexColumns { get; set; }
		List<bool> bSearchable { get; set; }
		List<bool> bSortable { get; set; }
		int iColumns { get; set; }
		int iDisplayLength { get; set; }
		int iDisplayStart { get; set; }
		List<int> iSortCol { get; set; }
		int iSortingCols { get; set; }
		List<string> sColumnNames { get; set; }
		int sEcho { get; set; }
		string sSearch { get; set; }
		List<string> sSearchValues { get; set; }
		List<string> sSortDir { get; set; }
	}
}
