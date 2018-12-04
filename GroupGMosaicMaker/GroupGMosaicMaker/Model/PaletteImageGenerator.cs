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
        public PixelBlock Pixels { get; set; }
        
        public Color AverageColor { get; set; }

        public PaletteImageGenerator()
        {
            this.Pixels = new PixelBlock();
        }

        /*
        public override async Task<WriteableBitmap> GenerateImageAsync()
        {
            var modifiedImage = new WriteableBitmap((int) this.Decoder.PixelWidth, (int) this.Decoder.PixelHeight);
            using (var writeStream = modifiedImage.PixelBuffer.AsStream())
            {
                var encoder = await BitmapEncoder.CreateForTranscodingAsync(writeStream, Decoder);
                encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Linear;
                encoder.BitmapTransform.ScaledWidth
                await writeStream.WriteAsync(this.sourcePixels, 0, this.sourcePixels.Length);
            }

            return modifiedImage;

            var encoder = await BitmapEncoder.CreateForTranscodingAsync(resizedStream, decoder);
            encoder.BitmapTransform.InterpolationMode = BitmapInterpolationMode.Linear;
            encoder.BitmapTransform.ScaledWidth = 
            encoder.BitmapTransform.ScaledHeight = height;
            await encoder.FlushAsync();
        }
        */

        public override async Task SetSourceAsync(IRandomAccessStream imageSource)
        {
            await base.SetSourceAsync(imageSource);

            this.assignPixels();
        }

        public async Task ScaleImage(int scaledWidth, int scaledHeight)
        {
            var convertedWidth = Convert.ToUInt32(scaledWidth);
            var convertedHeight = Convert.ToUInt32(scaledHeight);

            await this.assignSourcePixelsAsync(convertedWidth, convertedHeight);
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