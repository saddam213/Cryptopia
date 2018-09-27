using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Image
{
	public class CreateImageModel
	{
		public string Name { get; set; }
		public string Directory { get; set; }
		public bool CanResize { get; set; }
		public int MaxHeight { get; set; }
		public int MaxWidth { get; set; }
		public long MaxFileSize { get; set; }
		public bool PreserveAspectRatio { get; set; }
		public Stream FileStream { get; set; }
		public string Extention { get; set; }
	}
}
