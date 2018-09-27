using System.IO;
using ImageMagick;

namespace DoTheSpriteThing.Helpers
{
    public interface IMagickImageHelper
    {
        MagickImage Create(FileInfo file);

        MagickImage Create(byte[] data);
    }
}