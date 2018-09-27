using Cryptopia.Enums;
using System;
using System.Linq;

namespace Cryptopia.Common.Address
{
	public class DisplayAddressModel
	{
		public int CurrencyId { get; set; }
		public string Symbol { get; set; }
		public string Name { get; set; }
		public string DisplayName
		{
			get { return $"{Name}({Symbol})"; }
		}
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
		public string QrFormat { get; set; }
		public CurrencyType CurrencyType { get; set; }
		public AddressType AddressType { get; set; }
		public string Message { get; set; }
		public string MessageType { get; set; }
		public string Instructions { get; set; }

		public string QrCode
		{
			get
			{
				return string.IsNullOrEmpty(QrFormat)
					? AddressData
					: string.Format(QrFormat, AddressData, AddressData2, AddressData3);
			}
		}
		public string ReturnUrl { get; set; }
	}
}