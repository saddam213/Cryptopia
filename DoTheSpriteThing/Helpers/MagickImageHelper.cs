using System.IO;
using ImageMagick;

namespace DoTheSpriteThing.Helpers
{
    public class MagickImageHelper : IMagickImageHelper
    {
        public MagickImage Create(FileInfo file)
        {
            return new MagickImage(file);
        }

        public MagickImage Create(byte[] data)
        {
            return new MagickImage(data);
        }
    }
}