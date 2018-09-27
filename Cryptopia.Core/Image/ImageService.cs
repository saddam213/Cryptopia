using Cryptopia.Base;
using Cryptopia.Common.Image;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryptopia.Core.Image
{
	public class ImageService : IImageService
	{
		public Task<ValidateImageResultModel> ValidateImage(ValidateImageModel model)
		{
			try
			{
				if (model.FileStream.Length > model.MaxFileSize)
					return Task.FromResult(new ValidateImageResultModel(false, $"File too large, maximum file size is {model.MaxFileSize.GetBytesReadable()}"));

				using (var original = System.Drawing.Image.FromStream(model.FileStream))
				{
					if (!model.CanResize && (original.Width > model.MaxWidth || original.Height > model.MaxHeight))
						return Task.FromResult(new ValidateImageResultModel(false, $"Image too large, maximum image size is {model.MaxWidth} x {model.MaxHeight}"));
					
				}

				return Task.FromResult(new ValidateImageResultModel
				{
					Success = true,
					Message = "Successfully validates image.",
				});
			}
			catch (Exception)
			{
				return Task.FromResult(new ValidateImageResultModel(false, "An error occurred validating image file."));
			}
		}

		public async Task<CreateImageResultModel> SaveImage(CreateImageModel model)
		{
			try
			{
				if (model.FileStream.Length > model.MaxFileSize)
					return new CreateImageResultModel(false, $"File too large, maximum file size is {model.MaxFileSize.GetBytesReadable()}");

				var fileName = model.Name;
				if (string.IsNullOrEmpty(fileName))
					fileName = Path.GetRandomFileName();

				var ext = string.IsNullOrEmpty(model.Extention) ? ".PNG" : model.Extention;
				var destPath = Path.Combine(model.Directory, fileName) + ext;
				using (var original = System.Drawing.Image.FromStream(model.FileStream))
				{
					if (!model.CanResize && (original.Width > model.MaxWidth || original.Height > model.MaxHeight))
						return new CreateImageResultModel(false, $"Image too large, maximum image size is {model.MaxWidth} x {model.MaxHeight}");

					if (model.CanResize)
					{
						using (var processed = ResizeImage(original, model.MaxWidth, model.MaxHeight, model.PreserveAspectRatio))
						{
							processed.Save(destPath);
						}
					}
					else
					{
						using (var fileStream = File.Create(destPath))
						{
							model.FileStream.Seek(0, SeekOrigin.Begin);
							await model.FileStream.CopyToAsync(fileStream).ConfigureAwait(false);
						}
					}
				}
				return new CreateImageResultModel
				{
					Success = true,
					Name = fileName,
					Filename = destPath
				};
			}
			catch (Exception)
			{
				return new CreateImageResultModel(false, "An error occurred saving image file.");
			}
		}



		private static System.Drawing.Image ResizeImage(System.Drawing.Image imgPhoto, int width, int height, bool preserverAspectRatio)
		{
			if (!preserverAspectRatio)
			{
				return new Bitmap(imgPhoto, new Size(width, height));
			}

			var sourceWidth = imgPhoto.Width;
			var sourceHeight = imgPhoto.Height;
			float nPercent = 0;
			float nPercentW = 0;
			float nPercentH = 0;

			nPercentW = width / (float)sourceWidth;
			nPercentH = height / (float)sourceHeight;
			if (nPercentH < nPercentW)
			{
				nPercent = nPercentH;
			}
			else
			{
				nPercent = nPercentW;
			}

			var destWidth = Math.Min(width, (int)(sourceWidth * nPercent));
			var destHeight = Math.Min(height, (int)(sourceHeight * nPercent));

			var bmPhoto = new Bitmap(destWidth, destHeight, PixelFormat.Format32bppArgb);
			bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);
			using (var grPhoto = Graphics.FromImage(bmPhoto))
			{
				grPhoto.Clear(Color.Transparent);
				grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
				grPhoto.DrawImage(imgPhoto,
						new Rectangle(0, 0, destWidth, destHeight),
						new Rectangle(0, 0, sourceWidth, sourceHeight),
						GraphicsUnit.Pixel);
			}
			return bmPhoto;
		}

	}


}
