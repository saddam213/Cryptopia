using DoTheSpriteThing.Images.Interfaces;

namespace DoTheSpriteThing.Images
{
    /// <summary>
    /// A byte array image.
    /// </summary>
    public class HoverByteArrayImage : IHoverImage, IByteArrayImage
    {
        /// <summary>
        /// Create the byte array image.
        /// </summary>        
        /// <param name="imageData">The image data.</param>
        /// <param name="placeholderImageKey">The key of the placeholder image to use when image data is null.</param>        
        public HoverByteArrayImage(byte[] imageData, string placeholderImageKey)
        {
            ImageData = imageData;
            PlaceholderImageKey = placeholderImageKey;
        }

        /// <summary>
        /// Create the byte array image.
        /// </summary>        
        /// <param name="imageData">The image data.</param>        
        public HoverByteArrayImage(byte[] imageData)
        {
            ImageData = imageData;
        }

        /// <summary>
        /// The image data.
        /// </summary>
        public byte[] ImageData { get; }

        /// <summary>
        /// The key of the placeholder image to use when image data is null.
        /// </summary>
        public string PlaceholderImageKey { get; }
    }
}