using System;
using Cryptopia.Datatables.Models;

namespace Cryptopia.Datatables
{
	public abstract class DataTablesAttributeBase : Attribute
	{
		public abstract void ApplyTo(ColDef colDef, System.Reflection.PropertyInfo pi);
	}
}