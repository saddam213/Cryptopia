using DoTheSpriteThing.FileProcessors.Interfaces;
using ImageMagick;

namespace DoTheSpriteThing.FileProcessors
{
    internal class ImageProcessor : IImageProcessor
    {
        public void CreateSprite(MagickImageCollection spriteImages, string spriteFilename)
        {
            using (var result = spriteImages.AppendVertically())
            {
                result.Write(spriteFilename);
            }
        }
    }
}