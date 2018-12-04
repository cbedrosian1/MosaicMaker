﻿using System;
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
        protected byte[] sourcePixels;

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
                await writeStream.WriteAsync(this.sourcePixels, 0, this.sourcePixels.Length);
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

            await this.assignSourcePixelsAsync(this.Decoder.PixelWidth, this.Decoder.PixelHeight);
        }

        protected async Task assignSourcePixelsAsync(uint width, uint height)
        {
            var pixelData = await this.generatePixelDataAsync(width, height);
            this.sourcePixels = pixelData.DetachPixelData();
        }

        protected async Task<PixelDataProvider> generatePixelDataAsync(uint width, uint height)
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
                var r = this.sourcePixels[offset + 2];
                var g = this.sourcePixels[offset + 1];
                var b = this.sourcePixels[offset + 0];
                return Color.FromArgb(0, r, g, b);
            }

            return new Color();
        }

        protected void SetPixelColor(int x, int y, Color color)
        {
            var offset = this.CalculatePixelOffset(x, y);
            if (this.offsetIsValid(offset))
            {
                this.sourcePixels[offset + 2] = color.R;
                this.sourcePixels[offset + 1] = color.G;
                this.sourcePixels[offset + 0] = color.B;
            }
        }

        protected virtual int CalculatePixelOffset(int x, int y)
        {
            return (x * (int) this.Decoder.PixelWidth + y) * 4;
        }

        private bool offsetIsValid(int offset)
        {
            return offset + 2 < this.sourcePixels.Length;
        }

        protected bool CoordinatesAreValid(int x, int y)
        {
            var offset = this.CalculatePixelOffset(x, y);
            return this.offsetIsValid(offset);
        }

        #endregion
    }
}