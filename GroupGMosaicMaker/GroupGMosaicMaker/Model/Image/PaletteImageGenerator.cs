using System;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI;
using GroupGMosaicMaker.Model.Mosaic;

namespace GroupGMosaicMaker.Model.Image
{
    public class PaletteImageGenerator : ImageGenerator
    {
        #region Data members

        private uint scaledWidth;
        private uint scaledHeight;

        #endregion

        #region Properties

        public PixelBlock Pixels { get; set; }

        public Color AverageColor { get; set; }

        #endregion

        #region Constructors

        public PaletteImageGenerator()
        {
            this.Pixels = new PixelBlock();
        }

        #endregion

        #region Methods

        public override async Task SetSourceAsync(IRandomAccessStream imageSource)
        {
            await base.SetSourceAsync(imageSource);

            this.scaledWidth = Decoder.PixelWidth;
            this.scaledHeight = Decoder.PixelHeight;

            this.assignPixels();
        }

        public async Task ScaleImage(int scaledWidth, int scaledHeight)
        {
            var convertedWidth = Convert.ToUInt32(scaledWidth);
            var convertedHeight = Convert.ToUInt32(scaledHeight);

            this.scaledWidth = convertedWidth;
            this.scaledHeight = convertedHeight;

            await assignSourcePixelsAsync(convertedWidth, convertedHeight);
        }

        protected override int CalculatePixelOffset(int x, int y)
        {
            return (x * (int) this.scaledWidth + y) * 4;
        }

        private void assignPixels()
        {
            for (var i = 0; i < Decoder.PixelWidth; i++)
            {
                for (var j = 0; j < Decoder.PixelHeight; j++)
                {
                    var color = FindPixelColor(i, j);
                    this.Pixels.Add(color);
                }
            }
        }

        #endregion
    }
}