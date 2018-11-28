using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using GroupGMosaicMaker.Model;

namespace GroupGMosaicMaker.DataTier
{
    /// <summary>
    ///     Writes loaded image data to files.
    /// </summary>
    public static class ImageWriter
    {
        #region Methods

        /// <summary>
        ///     Writes the image from the given operator to the given file, asynchronously.
        /// </summary>
        /// <param name="imageOperator">The image operator (where the image comes from).</param>
        /// <param name="imageFile">The image file (where the image will be saved).</param>
        /// <returns>The completed asynchronous operation.</returns>
        public static async Task WriteImageAsync(ImageOperator imageOperator, StorageFile imageFile)
        {
            var stream = await imageFile.OpenAsync(FileAccessMode.ReadWrite);
            var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);

            var image = await imageOperator.GenerateImageAsync();

            var pixelStream = image.PixelBuffer.AsStream();
            var pixels = new byte[pixelStream.Length];
            await pixelStream.ReadAsync(pixels, 0, pixels.Length);

            encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore,
                (uint) image.PixelWidth,
                (uint) image.PixelHeight, imageOperator.Decoder.DpiX, imageOperator.Decoder.DpiY, pixels);
            await encoder.FlushAsync();

            stream.Dispose();
        }

        #endregion
    }
}