using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Image
{
	public class CreateImageResultModel
	{
		public CreateImageResultModel() { }
		public CreateImageResultModel(bool success, string message)
		{
			Success = success;
			Message = message;
		}

		public string Filename { get; set; }
		public bool Success { get; set; }
		public string Message { get; set; }
		public string Name { get; set; }
	}
}
