using System;
using System.Collections.Generic;
using Cryptopia.Datatables.Reflection;

namespace Cryptopia.Datatables.Util
{
	public class TransformTypeInfo
	{
		public static Dictionary<string, object> MergeTransformValuesIntoDictionary<TInput, TTransform>(
			Func<TInput, TTransform> transformInput, TInput tInput)
		{
			//get the the properties from the input as a dictionary
			var dict = DataTablesTypeInfo<TInput>.ToDictionary(tInput);

			//get the transform object
			var transform = transformInput(tInput);
			if (transform != null)
			{
				foreach (var propertyInfo in transform.GetType().GetProperties())
				{
					dict[propertyInfo.Name] = propertyInfo.GetValue(transform, null);
				}
			}
			return dict;
		}
	}
}