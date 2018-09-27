using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Web.Site.Helpers
{
	public static class ImageHelpers
	{
		public static string GetBase64String(this HttpPostedFileBase filestream, int maxWidth, int maxHeight)
		{
			try
			{
				using (var image = Image.FromStream(filestream.InputStream))
				{
					if (image.Width > maxWidth || image.Height > maxHeight)
					{
						using (var resizedImage = FixedSize(image, maxWidth, maxHeight))
						{
							return Base64FromImage(resizedImage);
						}
					}
					return Base64FromImage(image);
				}
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}

		public static string GetBase64String(this HttpPostedFileBase filestream)
		{
			try
			{
				using (var image = Image.FromStream(filestream.InputStream))
				{
					return Base64FromImage(image);
				}
			}
			catch (Exception)
			{
				return string.Empty;
			}
		}


		private static string Base64FromImage(Image image)
		{
			using (var memoryStream = new MemoryStream())
			{
				image.Save(memoryStream, ImageFormat.Png);
				memoryStream.Seek(0, SeekOrigin.Begin);
				using (var br = new BinaryReader(memoryStream))
				{
					var bytes = br.ReadBytes((int)memoryStream.Length);
					return $"data:png;base64,{Convert.ToBase64String(bytes, 0, bytes.Length)}";
				}
			}
		}

		private static Image FixedSize(Image imgPhoto, int Width, int Height)
		{
			int sourceWidth = imgPhoto.Width;
			int sourceHeight = imgPhoto.Height;
			int sourceX = 0;
			int sourceY = 0;
			int destX = 0;
			int destY = 0;

			float nPercent = 0;
			float nPercentW = 0;
			float nPercentH = 0;

			nPercentW = ((float)Width / (float)sourceWidth);
			nPercentH = ((float)Height / (float)sourceHeight);
			if (nPercentH < nPercentW)
			{
				nPercent = nPercentH;
				destX = System.Convert.ToInt16((Width -
											(sourceWidth * nPercent)) / 2);
			}
			else
			{
				nPercent = nPercentW;
				destY = System.Convert.ToInt16((Height -
											(sourceHeight * nPercent)) / 2);
			}

			int destWidth = (int)(sourceWidth * nPercent);
			int destHeight = (int)(sourceHeight * nPercent);

			Bitmap bmPhoto = new Bitmap(Width, Height,
												PixelFormat.Format32bppArgb);
			bmPhoto.SetResolution(imgPhoto.HorizontalResolution,
											 imgPhoto.VerticalResolution);

			Graphics grPhoto = Graphics.FromImage(bmPhoto);
			grPhoto.Clear(Color.Transparent);
			grPhoto.InterpolationMode =
							InterpolationMode.HighQualityBicubic;

			grPhoto.DrawImage(imgPhoto,
					new Rectangle(destX, destY, destWidth, destHeight),
					new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
					GraphicsUnit.Pixel);

			grPhoto.Dispose();
			return bmPhoto;
		}

	}
}
