using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using GroupGMosaicMaker.Model.Mosaic;

namespace GroupGMosaicMaker.Model.Image
{
    /// <summary>
    ///     ImageGenerator that models a palette image, capable of storage an average color and being scaled up or down in a
    ///     square.
    /// </summary>
    /// <seealso cref="GroupGMosaicMaker.Model.Image.ImageGenerator" />
    public class PaletteImageGenerator : ImageGenerator
    {
        #region Data members

        private uint scaledLength;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the pixel block associated with the image source.
        /// </summary>
        /// <value>
        ///     The pixel block.
        /// </value>
        public PixelBlock PixelBlock { get; }

        /// <summary>
        ///     Gets or sets the WritableBitmap associated with the image source
        /// </summary>
        ///
        /// <value>
        ///     The WriteableBitmap
        /// </value>
        public WriteableBitmap ThumbnailImage { get; set; }

        //TODO maybe rename this. Model shouldn't know about view and thumbnail image implies it knows maybe?

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PaletteImageGenerator"/> class.
        /// </summary>
        public PaletteImageGenerator()
        {
            this.PixelBlock = new PixelBlock();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes the <see cref="ImageGenerator" /> for use with the desired image source asynchronously.
        /// </summary>
        /// <param name="imageSource">The image source.</param>
        /// <returns>
        /// The completed asynchronous operation.
        /// </returns>
        public override async Task SetSourceAsync(IRandomAccessStream imageSource)
        {
            await base.SetSourceAsync(imageSource);

            this.scaledLength = Decoder.PixelWidth;

            this.assignPixels();
        }

        /// <summary>
        ///     Scales the image down into a square of the given new length.
        /// </summary>
        /// <param name="newLength">The new length.</param>
        /// <returns>The completed asynchronous operation.</returns>
        public async Task ScaleImageSquare(int newLength)
        {
            var convertedWidth = Convert.ToUInt32(newLength);
            this.scaledLength = convertedWidth;

            await AssignSourcePixelsAsync(convertedWidth, convertedWidth);
        }

        /// <summary>
        /// Calculates the pixel offset.
        /// </summary>
        /// <param name="x">The row.</param>
        /// <param name="y">The the column.</param>
        /// <returns>The pixel offset for accessing the source pixels.</returns>
        protected override int CalculatePixelOffset(int x, int y)
        {
            return (x * (int) this.scaledLength + y) * 4;
        }

        private void assignPixels()
        {
            for (var i = 0; i < Decoder.PixelWidth; i++)
            {
                for (var j = 0; j < Decoder.PixelHeight; j++)
                {
                    var color = FindPixelColor(i, j);
                    this.PixelBlock.Add(color);
                }
            }
        }

        #endregion
    }
}