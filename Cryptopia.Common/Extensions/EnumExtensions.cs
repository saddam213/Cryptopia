using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Extensions
{
	public static class EnumExtensions
	{
		/// <summary>
		/// Razor needs some help with localized enum display names
		/// </summary>
		/// <param name="enumValue"></param>
		/// <returns></returns>
		public static string GetDisplayName(this Enum enumValue)
		{
			return enumValue.GetType()
							.GetMember(enumValue.ToString())
							.First()
							.GetCustomAttribute<DisplayAttribute>()
							.GetName();
		}
	}
}
