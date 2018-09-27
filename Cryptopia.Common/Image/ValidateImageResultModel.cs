using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Image
{
	public class ValidateImageResultModel
	{
		public ValidateImageResultModel() { }
		public ValidateImageResultModel(bool success, string message)
		{
			Success = success;
			Message = message;
		}

		public bool Success { get; set; }
		public string Message { get; set; }
	}
}
