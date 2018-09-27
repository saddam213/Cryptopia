using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;

namespace Web.Site
{
	public static class ImageHelpers
    {
        public static byte[] ImageToBytes(string imagefilePath)
        {
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(imagefilePath))
            {
                using (var ms = new MemoryStream())
                {
                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    return ms.ToArray();
                }
            }
        }



        public static async Task SaveImageStreamToDiskAsync(Stream stream, string filename, string ext, string imageFolder = "", int width = 0, int height = 0)
        {
            await Task.Run(() =>
            {
                string newfilename = Path.Combine(HostingEnvironment.MapPath("~/Content/Images"), imageFolder, filename);
                using (var image = Image.FromStream(stream))
                {
                    // if no resize is required, or its a gif with the correct size just save to disk
                    if ((width == 0 && height == 0) || (ext.Equals("gif", StringComparison.OrdinalIgnoreCase) && width > 0 && height > 0 && image.Width <= width && image.Height <= height))
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        using (var fileStream = File.Create(string.Format("{0}.{1}", newfilename, ext)))
                        {
                            stream.CopyTo(fileStream);
                        }
                    }
                    else
                    {
                        using (var newImage = new Bitmap(image, new Size(width == 0 ? image.Width : width, height == 0 ? image.Height : height)))
                        {
                            newImage.Save(newfilename + ".png", ImageFormat.Png);
                        }
                    }
                }
            });
        }

        public static async Task SaveImageUrlToDiskAsync(string url, string filename, string imageFolder = "", int width = 0, int height = 0)
        {
            var ext = Path.GetExtension(url).Replace(".", "");
            using (HttpClient client = new HttpClient())
            {
                using (var stream = await client.GetStreamAsync(url))
                {
                    if (stream.Length > (1 * 1024 * 1024))
                    {
                        throw new CustomValidationException("Avatar image must be less than 1MB in size.");
                    }

                    await SaveImageStreamToDiskAsync(stream, filename, ext, imageFolder, width, height);
                }
            }
        }

        public static async Task SaveAvatarFileToDiskAsync(HttpPostedFileBase httpImage, string username)
        {
            var ext = httpImage.ContentType.Split('/').Last();
            if (httpImage.ContentLength > (1 * 1024 * 1024))
            {
                throw new CustomValidationException("Avatar image must be less than 1MB in size.");
            }
            await SaveImageStreamToDiskAsync(httpImage.InputStream, username, ext, "Avatar", 60, 60);
        }

        public static async Task SaveAvatarUrlToDiskAsync(string avatarUrl, string username)
        {
            await SaveImageUrlToDiskAsync(avatarUrl, username, "Avatar", 60, 60);
        }



        public static void RemoveAvatarFromDiskAsync(string username)
        {
            string serverPath = HostingEnvironment.MapPath("~/Content/Images/Avatar");
            string filename = string.Format("{0}\\{1}", serverPath, username);
            if (File.Exists(filename + ".gif"))
            {
                File.Delete(filename + ".gif");
            }

            if (File.Exists(filename + ".png"))
            {
                File.Delete(filename + ".png");
            }
        }


        public static async Task<string> SaveMarketItemImageToDiskAsync(this HttpPostedFileBase httpImage, bool mainImage)
        {
            if (httpImage == null)
            {
                return string.Empty;
            }

            if (httpImage.ContentLength > (5 * 1024 * 1024))
            {
                return "Failed To save image, image must be less than 5MB in size.";
            }

            return await Task.Run(() =>
            {
                try
                {
                    string filename = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ".png";
                    string directory = "~/Content/Images/MarketPlace";
                    string newfilename = Path.Combine(HostingEnvironment.MapPath(directory), filename);
                    using (var image = Image.FromStream(httpImage.InputStream))
                    {
                        if (mainImage)
                        {
                            using (var newImage = FixedSize(image, 200, 150))
                            {
                                newImage.Save(newfilename.Replace(".png", "_Thumb.png"), ImageFormat.Png);
                            }
                        }
                        using (var newImage = FixedSize(image, 600, 400))
                        {
                            newImage.Save(newfilename, ImageFormat.Png);
                        }
                    }
                    return string.Format("/Content/Images/MarketPlace/{0}", filename);
                }
                catch (Exception ex)
                {
                    return string.Format("Failed To save image, {0}", ex.Message);
                }
            });
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

    public class CustomValidationException : Exception
    {
        public CustomValidationException(string message)
            : base(message)
        {

        }
    }
}