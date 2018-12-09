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

        /// <summary>
        ///     Assigns the source pixels asynchronous.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>The completed asynchronous operation.</returns>
        protected async Task AssignSourcePixelsAsync(uint width, uint height)
        {
            var pixelData = await this.GeneratePixelDataAsync(width, height);
            this.SourcePixels = pixelData.DetachPixelData();
        }

        /// <summary>
        ///     Generates the pixel data asynchronous.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns></returns>
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

        /// <summary>
        ///     Finds the color of the pixel.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>The color of the pixel.</returns>
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

        /// <summary>
        ///     Sets the color of the pixel.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="color">The color.</param>
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

        /// <summary>
        ///     Calculates the pixel offset.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>The pixel offset</returns>
        protected virtual int CalculatePixelOffset(int x, int y)
        {
            return (x * (int) this.Decoder.PixelWidth + y) * 4;
        }

        private bool offsetIsValid(int offset)
        {
            return offset + 2 < this.SourcePixels.Length;
        }

        /// <summary>Checks if Coordinates are valid.</summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>True if coordinates are valid and false if not</returns>
        protected bool CoordinatesAreValid(int x, int y)
        {
            var offset = this.CalculatePixelOffset(x, y);
            return this.offsetIsValid(offset);
        }

        #endregion
    }
}