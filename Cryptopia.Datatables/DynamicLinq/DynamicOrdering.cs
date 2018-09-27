using System.Linq.Expressions;

namespace Cryptopia.Datatables.DynamicLinq
{
	internal class DynamicOrdering
	{
		public Expression Selector;
		public bool Ascending;
	}
}