using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace GroupGMosaicMaker.Model.Image
{
    /// <summary>
    ///     Base class that allows for generating image data.
    /// </summary>
    public class ImageGenerator
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

        /// <summary>
        ///     Generates the image.
        /// </summary>
        /// <returns>The image</returns>
        public virtual async Task<WriteableBitmap> GenerateImageAsync()
        {
            var modifiedImage = new WriteableBitmap((int) this.Decoder.PixelWidth, (int) this.Decoder.PixelHeight);
            using (var writeStream = modifiedImage.PixelBuffer.AsStream())
            {
                await writeStream.WriteAsync(this.SourcePixels, 0, this.SourcePixels.Length);
            }

            return modifiedImage;
        }

        /// <summary>
        ///     Initializes the <see cref="ImageGenerator" /> for use with the desired image source asynchronously.
        /// </summary>
        /// <param name="imageSource">The image source.</param>
        /// <returns>The completed asynchronous operation.</returns>
        public virtual async Task SetSourceAsync(IRandomAccessStream imageSource)
        {
            this.Decoder = await BitmapDecoder.CreateAsync(imageSource);

            await this.AssignSourcePixelsAsync(this.Decoder.PixelWidth, this.Decoder.PixelHeight);
        }

        protected async Task AssignSourcePixelsAsync(uint width, uint height)
        {
            var pixelData = await this.GeneratePixelDataAsync(width, height);
            this.SourcePixels = pixelData.DetachPixelData();
        }

        protected async Task<PixelDataProvider> GeneratePixelDataAsync(uint width, uint height)
        {
            var transform = this.generateBitmapTransform(width, height);
            var pixelData = await this.Decoder.GetPixelDataAsync(
                BitmapPixelFormat.Bgra8,
                BitmapAlphaMode.Straight,
                transform,
                ExifOrientationMode.IgnoreExifOrientation,
                ColorManagementMode.DoNotColorManage
            );

            return pixelData;
        }

        private BitmapTransform generateBitmapTransform(uint width, uint height)
        {
            var transform = new BitmapTransform {
                ScaledWidth = width,
                ScaledHeight = height
            };

            return transform;
        }

        public Color FindPixelColor(int x, int y)
        {
            var offset = this.CalculatePixelOffset(x, y);
            if (this.offsetIsValid(offset))
            {
                var r = this.SourcePixels[offset + 2];
                var g = this.SourcePixels[offset + 1];
                var b = this.SourcePixels[offset + 0];
                return Color.FromArgb(0, r, g, b);
            }

            return new Color();
        }

        protected void SetPixelColor(int x, int y, Color color)
        {
            var offset = this.CalculatePixelOffset(x, y);
            if (this.offsetIsValid(offset))
            {
                this.SourcePixels[offset + 2] = color.R;
                this.SourcePixels[offset + 1] = color.G;
                this.SourcePixels[offset + 0] = color.B;
            }
        }

        protected virtual int CalculatePixelOffset(int x, int y)
        {
            return (y * (int) this.Decoder.PixelWidth + x) * 4;
        }

        private bool offsetIsValid(int offset)
        {
            return offset + 2 < this.SourcePixels.Length;
        }

        protected bool CoordinatesAreValid(int x, int y)
        {
            var offset = this.CalculatePixelOffset(x, y);
            return this.offsetIsValid(offset);
        }

        #endregion
    }
}