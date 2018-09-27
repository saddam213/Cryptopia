using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Common.Image
{
	public interface IImageService
	{
		Task<CreateImageResultModel> SaveImage(CreateImageModel model);
		Task<ValidateImageResultModel> ValidateImage(ValidateImageModel model);
	}
}
