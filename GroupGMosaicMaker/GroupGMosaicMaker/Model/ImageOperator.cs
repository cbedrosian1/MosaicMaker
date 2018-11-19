using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace GroupGMosaicMaker.Model
{
    // TODO Better class name
    /// <summary>
    ///     Base class that allows for reading image data.
    /// </summary>
    public class ImageOperator
    {
        #region Data members

        /// <summary>
        ///     The source pixels of the image.
        /// </summary>
        protected byte[] SourcePixels;

        #endregion

        #region Properties

        public BitmapDecoder Decoder { get; private set; }

        #endregion

        #region Constructors

        protected ImageOperator()
        {
        }

        #endregion

        #region Methods

        private async Task initializeAsync(IRandomAccessStream imageSource)
        {
            this.Decoder = await BitmapDecoder.CreateAsync(imageSource);

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
        /// <param name="imageSource">The image source stream.</param>
        /// <returns>A new <see cref="ImageOperator" /> instance.</returns>
        public static async Task<ImageOperator> CreateAsync(IRandomAccessStream imageSource)
        {
            var imageOperator = new ImageOperator();
            await imageOperator.initializeAsync(imageSource);

            return imageOperator;
        }

        #endregion
    }
}