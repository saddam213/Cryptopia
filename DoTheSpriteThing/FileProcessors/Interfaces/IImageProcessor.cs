using ImageMagick;

namespace DoTheSpriteThing.FileProcessors.Interfaces
{
    internal interface IImageProcessor
    {
        void CreateSprite(MagickImageCollection spriteImages, string spriteFilename);
    }
}