using System.IO;
using DoTheSpriteThing.Images.Interfaces;

namespace DoTheSpriteThing.Images
{
    /// <summary>
    /// A file image.
    /// </summary>
    public class HoverFileImage : IHoverImage, IFileImage
    {
        /// <summary>
        /// Create the file image.
        /// </summary>
        /// <param name="filePath">The path of the image file.</param>
        /// <param name="placeholderImageKey">The key of the placeholder image to use when image data is null.</param>        
        public HoverFileImage(FileInfo filePath, string placeholderImageKey)
        {
            FilePath = filePath;
            PlaceholderImageKey = placeholderImageKey;
        }

        /// <summary>
        /// Create the file image.
        /// </summary>
        /// <param name="filePath">The path of the image file.</param>        
        public HoverFileImage(FileInfo filePath)
        {
            FilePath = filePath;
        }

        public FileInfo FilePath { get; }

        public string PlaceholderImageKey { get; }
    }
}