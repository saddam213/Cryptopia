using Cryptopia.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Address
{
	public class AddressModel
	{
		public int CurrencyId { get; set; }
		public string Name { get; set; }
		public string Symbol { get; set; }
		public string QrFormat { get; set; }
		public CurrencyType CurrencyType { get; set; }
		public string AddressData { get; set; }
		public string AddressData2 { get; set; }
		public string AddressData3
		{
			get
			{
				return string.IsNullOrEmpty(AddressData)
					? string.Empty
					: new String(AddressData.Skip(3).Take(8).ToArray()).ToUpper();
			}
		}
		public string QrCode
		{
			get
			{
				return string.IsNullOrEmpty(QrFormat)
					? AddressData
					: string.Format(QrFormat, AddressData, AddressData2, AddressData3);
			}
		}
		public string ErrorMessage { get; set; }
		public AddressType AddressType { get; set; }
	}
}
