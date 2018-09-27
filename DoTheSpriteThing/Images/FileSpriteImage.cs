using System.IO;
using DoTheSpriteThing.Images.Interfaces;

namespace DoTheSpriteThing.Images
{
    /// <summary>
    /// A file image.
    /// </summary>
    public class FileSpriteImage : ISpriteImage, IFileImage
    {
        /// <summary>
        /// Create the file image.
        /// </summary>
        /// <param name="filePath">The path of the image file.</param>
        /// <param name="placeholderImageKey">The key of the placeholder image to use when image data is null.</param>        
        public FileSpriteImage(FileInfo filePath, string placeholderImageKey)
        {
            FilePath = filePath;
            PlaceholderImageKey = placeholderImageKey;
            Resize = false;
            Quality = 100;
        }

        /// <summary>
        /// Create the file image.
        /// </summary>
        /// <param name="filePath">The path of the image file.</param>
        /// <param name="placeholderImageKey">The key of the placeholder image to use when image data is null.</param>
        /// <param name="quality">The quality of the image.</param>        
        public FileSpriteImage(FileInfo filePath, string placeholderImageKey, int quality)
        {
            FilePath = filePath;
            PlaceholderImageKey = placeholderImageKey;
            Resize = false;
            Quality = quality;
        }

        /// <summary>
        /// Create the file image.
        /// </summary>
        /// <param name="filePath">The path of the image file.</param>
        /// <param name="placeholderImageKey">The key of the placeholder image to use when image data is null.</param>   
        /// <param name="hoverImage">The image to display when hovering over the image.</param>             
        public FileSpriteImage(FileInfo filePath, string placeholderImageKey, IHoverImage hoverImage)
        {
            FilePath = filePath;
            PlaceholderImageKey = placeholderImageKey;
            HoverImage = hoverImage;
            Resize = false;
            Quality = 100;
        }

        /// <summary>
        /// Create the file image.
        /// </summary>
        /// <param name="filePath">The path of the image file.</param>
        /// <param name="placeholderImageKey">The key of the placeholder image to use when image data is null.</param>   
        /// <param name="hoverImage">The image to display when hovering over the image.</param>
        /// <param name="quality">The quality of the image.</param>             
        public FileSpriteImage(FileInfo filePath, string placeholderImageKey, IHoverImage hoverImage, int quality)
        {
            FilePath = filePath;
            PlaceholderImageKey = placeholderImageKey;
            HoverImage = hoverImage;
            Resize = false;
            Quality = quality;
        }

        /// <summary>
        /// Create the file image.
        /// </summary>
        /// <param name="filePath">The path of the image file.</param>        
        public FileSpriteImage(FileInfo filePath)
        {
            FilePath = filePath;
            Resize = false;
            Quality = 100;
        }

        /// <summary>
        /// Create the file image.
        /// </summary>
        /// <param name="filePath">The path of the image file.</param>
        /// <param name="quality">The quality of the image.</param>        
        public FileSpriteImage(FileInfo filePath, int quality)
        {
            FilePath = filePath;
            Resize = false;
            Quality = quality;
        }

        /// <summary>
        /// Create the file image.
        /// </summary>
        /// <param name="filePath">The path of the image file.</param>        
        /// <param name="hoverImage">The image to display when hovering over the image.</param>        
        public FileSpriteImage(FileInfo filePath, IHoverImage hoverImage)
        {
            Resize = false;
            FilePath = filePath;
            HoverImage = hoverImage;
            Quality = 100;
        }

        /// <summary>
        /// Create the file image.
        /// </summary>
        /// <param name="filePath">The path of the image file.</param>        
        /// <param name="hoverImage">The image to display when hovering over the image.</param>
        /// <param name="quality">The quality of the image.</param>        
        public FileSpriteImage(FileInfo filePath, IHoverImage hoverImage, int quality)
        {
            Resize = false;
            FilePath = filePath;
            HoverImage = hoverImage;
            Quality = quality;
        }

        /// <summary>
        /// Create the file image.
        /// </summary>
        /// <param name="filePath">The path of the image file.</param>
        /// <param name="placeholderImageKey">The key of the placeholder image to use when image data is null.</param>        
        /// <param name="resizeToHeight">The height in pixels to resize the image to.</param>
        /// <param name="resizeToWidth">The width in pixels to resize the image to.</param>
        public FileSpriteImage(FileInfo filePath, string placeholderImageKey, int resizeToHeight, int resizeToWidth)
        {
            Resize = true;
            FilePath = filePath;
            PlaceholderImageKey = placeholderImageKey;
            ResizeToHeight = resizeToHeight;
            ResizeToWidth = resizeToWidth;
            Quality = 100;
        }

        /// <summary>
        /// Create the file image.
        /// </summary>
        /// <param name="filePath">The path of the image file.</param>
        /// <param name="placeholderImageKey">The key of the placeholder image to use when image data is null.</param>        
        /// <param name="resizeToHeight">The height in pixels to resize the image to.</param>
        /// <param name="resizeToWidth">The width in pixels to resize the image to.</param>
        /// <param name="quality">The quality of the image.</param>
        public FileSpriteImage(FileInfo filePath, string placeholderImageKey, int resizeToHeight, int resizeToWidth, int quality)
        {
            Resize = true;
            FilePath = filePath;
            PlaceholderImageKey = placeholderImageKey;
            ResizeToHeight = resizeToHeight;
            ResizeToWidth = resizeToWidth;
            Quality = quality;
        }

        /// <summary>
        /// Create the file image.
        /// </summary>
        /// <param name="filePath">The path of the image file.</param>
        /// <param name="placeholderImageKey">The key of the placeholder image to use when image data is null.</param>        
        /// <param name="resizeToHeight">The height in pixels to resize the image to.</param>
        /// <param name="resizeToWidth">The width in pixels to resize the image to.</param>
        /// <param name="hoverImage">The image to display when hovering over the image.</param>        
        public FileSpriteImage(FileInfo filePath, string placeholderImageKey, int resizeToHeight, int resizeToWidth, IHoverImage hoverImage)
        {
            Resize = true;
            FilePath = filePath;
            PlaceholderImageKey = placeholderImageKey;
            ResizeToHeight = resizeToHeight;
            ResizeToWidth = resizeToWidth;
            HoverImage = hoverImage;
            Quality = 100;
        }

        /// <summary>
        /// Create the file image.
        /// </summary>
        /// <param name="filePath">The path of the image file.</param>
        /// <param name="placeholderImageKey">The key of the placeholder image to use when image data is null.</param>        
        /// <param name="resizeToHeight">The height in pixels to resize the image to.</param>
        /// <param name="resizeToWidth">The width in pixels to resize the image to.</param>
        /// <param name="hoverImage">The image to display when hovering over the image.</param>
        /// <param name="quality">The quality of the image.</param>        
        public FileSpriteImage(FileInfo filePath, string placeholderImageKey, int resizeToHeight, int resizeToWidth, IHoverImage hoverImage, int quality)
        {
            Resize = true;
            FilePath = filePath;
            PlaceholderImageKey = placeholderImageKey;
            ResizeToHeight = resizeToHeight;
            ResizeToWidth = resizeToWidth;
            HoverImage = hoverImage;
            Quality = quality;
        }

        /// <summary>
        /// Create the file image.
        /// </summary>
        /// <param name="filePath">The path of the image file.</param>        
        /// <param name="resizeToHeight">The height in pixels to resize the image to.</param>
        /// <param name="resizeToWidth">The width in pixels to resize the image to.</param>        
        public FileSpriteImage(FileInfo filePath, int resizeToHeight, int resizeToWidth)
        {
            Resize = true;
            FilePath = filePath;
            ResizeToHeight = resizeToHeight;
            ResizeToWidth = resizeToWidth;
            Quality = 100;
        }

        /// <summary>
        /// Create the file image.
        /// </summary>
        /// <param name="filePath">The path of the image file.</param>        
        /// <param name="resizeToHeight">The height in pixels to resize the image to.</param>
        /// <param name="resizeToWidth">The width in pixels to resize the image to.</param>
        /// <param name="quality">The quality of the image.</param>        
        public FileSpriteImage(FileInfo filePath, int resizeToHeight, int resizeToWidth, int quality)
        {
            Resize = true;
            FilePath = filePath;
            ResizeToHeight = resizeToHeight;
            ResizeToWidth = resizeToWidth;
            Quality = quality;
        }

        /// <summary>
        /// Create the file image.
        /// </summary>
        /// <param name="filePath">The path of the image file.</param>        
        /// <param name="resizeToHeight">The height in pixels to resize the image to.</param>
        /// <param name="resizeToWidth">The width in pixels to resize the image to.</param>
        /// <param name="hoverImage">The image to display when hovering over the image.</param>        
        public FileSpriteImage(FileInfo filePath, int resizeToHeight, int resizeToWidth, IHoverImage hoverImage)
        {
            Resize = true;
            FilePath = filePath;
            ResizeToHeight = resizeToHeight;
            ResizeToWidth = resizeToWidth;
            HoverImage = hoverImage;
            Quality = 100;
        }

        /// <summary>
        /// Create the file image.
        /// </summary>
        /// <param name="filePath">The path of the image file.</param>        
        /// <param name="resizeToHeight">The height in pixels to resize the image to.</param>
        /// <param name="resizeToWidth">The width in pixels to resize the image to.</param>
        /// <param name="hoverImage">The image to display when hovering over the image.</param>
        /// <param name="quality">The quality of the image.</param>        
        public FileSpriteImage(FileInfo filePath, int resizeToHeight, int resizeToWidth, IHoverImage hoverImage, int quality)
        {
            Resize = true;
            FilePath = filePath;
            ResizeToHeight = resizeToHeight;
            ResizeToWidth = resizeToWidth;
            HoverImage = hoverImage;
            Quality = quality;
        }

        public FileInfo FilePath { get; set; }

        public IHoverImage HoverImage { get; }

        public string Key => Path.GetFileName(FilePath.FullName).Replace(".", "-");

        public string PlaceholderImageKey { get; }

        public bool Resize { get; }

        public int ResizeToHeight { get; }

        public int ResizeToWidth { get; }

        public int Quality { get; }
    }
}