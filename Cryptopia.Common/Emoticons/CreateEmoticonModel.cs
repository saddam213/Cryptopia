using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Emoticons
{
	public class CreateEmoticonModel
	{
		[Required]
		public string Category { get; set; }

		[Required]
		public string Name { get; set; }

		public string Code { get; set; }

		public bool ForceResize { get; set; }

		public Stream FileStream { get; set; }
	}
}
