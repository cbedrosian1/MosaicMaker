using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace GroupGMosaicMaker.Model
{
    /// <summary>
    ///     Base class that allows for generating image data.
    /// </summary>
    public abstract class ImageOperator
    {
        #region Data members

        /// <summary>
        ///     The source pixels of the image.
        /// </summary>
        protected byte[] SourcePixels;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the decoder of the image.
        /// </summary>
        /// <value>
        ///     The decoder.
        /// </value>
        public BitmapDecoder Decoder { get; protected set; }

        #endregion

        #region Methods

        public abstract Task SetSourceAsync(IRandomAccessStream imageSource);

        /// <summary>
        ///     Generates the image.
        /// </summary>
        /// <returns>The image</returns>
        public async Task<WriteableBitmap> GenerateImageAsync()
        {
            var modifiedImage = new WriteableBitmap((int) this.Decoder.PixelWidth, (int) this.Decoder.PixelHeight);
            using (var writeStream = modifiedImage.PixelBuffer.AsStream())
            {
                await writeStream.WriteAsync(this.SourcePixels, 0, this.SourcePixels.Length);
            }

            return modifiedImage;
        }

        #endregion
    }
}