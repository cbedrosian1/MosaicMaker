using Windows.UI;
using GroupGMosaicMaker.Extensions;
using GroupGMosaicMaker.Model.Image;

namespace GroupGMosaicMaker.Model.Mosaic
{
    /// <summary>Contains basic functionality for creating a mosaic</summary>
    public abstract class MosaicMaker : ImageGenerator
    {
        #region Data members

        /// <summary>The halfway point between black and white</summary>
        protected const double HalfOfMaxColorChannel = 127.5;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the length of the block.
        /// </summary>
        /// <value>
        ///     The length of the block.
        /// </value>
        public int BlockLength { get; set; }

        #endregion

        #region Constructors

        #endregion

        #region Methods

        /// <summary>
        ///     Generates the mosaic onto the source image.
        /// </summary>
        public void GenerateMosaic()
        {
            for (var x = 0; x < Decoder.PixelHeight; x += this.BlockLength)
            {
                for (var y = 0; y < Decoder.PixelWidth; y += this.BlockLength)
                {
                    this.GenerateMosaicBlock(x, y);
                }
            }
        }

        /// <summary>
        ///     Generates the (x, y)'th block of the mosaic. Override this method to change the functionality of how each block is
        ///     generated.
        /// </summary>
        /// <param name="x">The row.</param>
        /// <param name="y">The column.</param>
        protected abstract void GenerateMosaicBlock(int x, int y);

        /// <summary>
        ///     Converts the mosaic to black and white.
        /// </summary>
        public abstract void ConvertToBlackAndWhite();

        /// <summary>
        ///     Converts the pixel to black and white.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        protected void ConvertPixelToBlackAndWhite(int x, int y)
        {
            var color = FindPixelColor(x, y);
            var average = color.CalculateAverageRgbChannelValue();
            if (average > HalfOfMaxColorChannel)
            {
                SetPixelColor(x, y, Colors.White);
            }
            else
            {
                SetPixelColor(x, y, Colors.Black);
            }
        }

        #endregion
    }
}