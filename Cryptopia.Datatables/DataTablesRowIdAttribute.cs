using System.Reflection;
using Cryptopia.Datatables.Models;

namespace Cryptopia.Datatables
{
	public class DataTablesRowIdAttribute : DataTablesAttributeBase
	{
		public bool EmitAsColumnName { get; set; }

		public override void ApplyTo(ColDef colDef, PropertyInfo pi)
		{
			// This attribute does not affect rendering
		}

		public DataTablesRowIdAttribute()
		{
			EmitAsColumnName = true;
		}
	}
}