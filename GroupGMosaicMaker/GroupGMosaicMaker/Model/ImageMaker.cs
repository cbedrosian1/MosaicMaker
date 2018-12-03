using System;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI;

namespace GroupGMosaicMaker.Model
{
    /// <summary>
    ///     Concrete <see cref="ImageGenerator" /> that allows for creating new objects of it's type.
    /// </summary>
    /// <seealso cref="ImageGenerator" />
    public class ImageMaker : ImageGenerator
    {
        #region Methods

        /// <summary>
        /// Initializes the <see cref="ImageMaker"/> for use with the desired image source asynchronously.
        /// </summary>
        /// <param name="imageSource">The image source.</param>
        /// <returns>The completed asynchronous operation.</returns>
        public override async Task SetSourceAsync(IRandomAccessStream imageSource)
        {
            Decoder = await BitmapDecoder.CreateAsync(imageSource);

            var pixelData = await this.generatePixelDataAsync();
            SourcePixels = pixelData.DetachPixelData();
        }

        private async Task<PixelDataProvider> generatePixelDataAsync()
        {
            var transform = this.generateBitmapTransform();
            var pixelData = await Decoder.GetPixelDataAsync(
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
                ScaledWidth = Convert.ToUInt32(Decoder.PixelWidth),
                ScaledHeight = Convert.ToUInt32(Decoder.PixelHeight)
            };

            return transform;
        }

        protected Color GetPixelBgra8(int x, int y)
        {
            var offset = this.CalculatePixelOffset(x, y);
            if (this.offsetIsValid(offset))
            {
                var r = SourcePixels[offset + 2];
                var g = SourcePixels[offset + 1];
                var b = SourcePixels[offset + 0];
                return Color.FromArgb(0, r, g, b);
            }

            return new Color();
        }

        protected void SetPixelBgra8(int x, int y, Color color)
        {
            var offset = this.CalculatePixelOffset(x, y);
            if (this.offsetIsValid(offset))
            {
                SourcePixels[offset + 2] = color.R;
                SourcePixels[offset + 1] = color.G;
                SourcePixels[offset + 0] = color.B;
            }
        }

        protected int CalculatePixelOffset(int x, int y)
        {
            return (x * (int) Decoder.PixelWidth + y) * 4;
        }

        private bool offsetIsValid(int offset)
        {
            return offset + 2 < SourcePixels.Length;
        }

        protected bool CoordinatesAreValid(int x, int y)
        {
            var offset = this.CalculatePixelOffset(x, y);
            return this.offsetIsValid(offset);
        }

        #endregion
    }
}