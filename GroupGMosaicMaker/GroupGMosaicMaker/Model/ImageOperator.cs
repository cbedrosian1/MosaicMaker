using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;

namespace GroupGMosaicMaker.Model
{
    /// <summary>
    ///     Base class that allows for reading image data.
    /// </summary>
    public class ImageOperator
    {
        #region Data members

        /// <summary>
        ///     The decoder for accessing image data.
        /// </summary>
        protected readonly BitmapDecoder Decoder;

        /// <summary>
        ///     The source pixels of the image.
        /// </summary>
        protected byte[] SourcePixels;

        #endregion

        #region Constructors

        protected ImageOperator(BitmapDecoder decoder)
        {
            this.Decoder = decoder;
        }

        #endregion

        #region Methods

        private async Task initializeAsync()
        {
            var pixelData = await this.generatePixelDataAsync();
            this.SourcePixels = pixelData.DetachPixelData();
        }

        private async Task<PixelDataProvider> generatePixelDataAsync()
        {
            var transform = this.generateBitmapTransform();
            var pixelData = await this.Decoder.GetPixelDataAsync(
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Straight,
                transform,
                ExifOrientationMode.IgnoreExifOrientation,
                ColorManagementMode.DoNotColorManage
            );

            return pixelData;
        }

        private BitmapTransform generateBitmapTransform()
        {
            var transform = new BitmapTransform {
                ScaledWidth = Convert.ToUInt32(this.Decoder.PixelWidth),
                ScaledHeight = Convert.ToUInt32(this.Decoder.PixelHeight)
            };

            return transform;
        }

        // TODO Consider making this method protected as development continues, or abstracting this method out somewhere else.
        /// <summary>
        ///     Generates the modified image.
        /// </summary>
        /// <returns>The modified image</returns>
        public async Task<WriteableBitmap> GenerateModifiedImageAsync()
        {
            var modifiedImage = new WriteableBitmap((int) this.Decoder.PixelWidth, (int) this.Decoder.PixelHeight);
            using (var writeStream = modifiedImage.PixelBuffer.AsStream())
            {
                await writeStream.WriteAsync(this.SourcePixels, 0, this.SourcePixels.Length);
            }

            return modifiedImage;
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="ImageOperator" /> class asynchronously.
        /// </summary>
        /// <param name="decoder">The decoder which contains the image data to be used.</param>
        /// <returns>A new <see cref="ImageOperator" /> instance.</returns>
        public static async Task<ImageOperator> CreateAsync(BitmapDecoder decoder)
        {
            var imageOperator = new ImageOperator(decoder);
            await imageOperator.initializeAsync();

            return imageOperator;
        }

        #endregion
    }
}