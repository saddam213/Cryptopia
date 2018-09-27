using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Cryptopia.Datatables.Reflection
{
	public static class StaticReflectionHelper
	{
		public static MethodInfo MethodInfo(this Expression method)
		{
			var lambda = method as LambdaExpression;
			if (lambda == null) throw new ArgumentNullException("method");
			MethodCallExpression methodExpr = null;
			if (lambda.Body.NodeType == ExpressionType.Call)
				methodExpr = lambda.Body as MethodCallExpression;

			if (methodExpr == null) throw new ArgumentNullException("method");
			return methodExpr.Method;
		}

	}
}