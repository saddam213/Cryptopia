﻿namespace Cryptopia.Infrastructure.Common.DataTables
{
	public class DataTablesResponse
	{
		public int iTotalRecords { get; set; }
		public int iTotalDisplayRecords { get; set; }
		public int sEcho { get; set; }
		public object[] aaData { get; set; }
	}
}
