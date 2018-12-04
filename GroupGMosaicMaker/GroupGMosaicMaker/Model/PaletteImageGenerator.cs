using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace GroupGMosaicMaker.Model
{
    public class PaletteImageGenerator : ImageGenerator
    {
        private uint scaledWidth;
        private uint scaledHeight;

        public PixelBlock Pixels { get; set; }
        
        public Color AverageColor { get; set; }

        public PaletteImageGenerator()
        {
            this.Pixels = new PixelBlock();
        }

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

            await this.assignSourcePixelsAsync(convertedWidth, convertedHeight);
        }

        protected override int CalculatePixelOffset(int x, int y)
        {
            return (x * (int) this.scaledWidth + y) * 4;
        }

        private void assignPixels()
        {
            for (int i = 0; i < Decoder.PixelWidth; i++)
            {
                for (int j = 0; j < Decoder.PixelHeight; j++)
                {
                    var color = FindPixelColor(i, j);
                    this.Pixels.Add(color);
                }
            }
        }
    }
}