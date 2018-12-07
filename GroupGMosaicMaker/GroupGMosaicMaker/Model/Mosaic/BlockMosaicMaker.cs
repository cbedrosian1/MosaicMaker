using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using GroupGMosaicMaker.Model.Image;

namespace GroupGMosaicMaker.Model.Mosaic
{
    /// <summary>
    ///     Responsible for generating solid block mosaics from the image source, of varying block sizes.
    /// </summary>
    /// <seealso cref="GroupGMosaicMaker.Model.Image.ImageGenerator" />
    public class BlockMosaicMaker : ImageGenerator
    {

        public const double NumberOfColorValues = 3.0;
        public const double HalfBetweenBlackAndWhite = 127.5; //TODO ok that these are public to use in picturemosaic?

        #region Properties

        /// <summary>
        ///     Gets or sets the length of the block.
        /// </summary>
        /// <value>
        ///     The length of the block.
        /// </value>
        public int BlockLength { get; set; }

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
        protected virtual void GenerateMosaicBlock(int x, int y)
        {
            var currentBlock = this.FindSingleBlock(x, y);
            var color = currentBlock.CalculateAverageColor();

            this.assignColorToBlock(x, y, color);
        }

        /// <summary>
        ///     Finds a single block from the given x and y coordinate, according to the block length.
        /// </summary>
        /// <param name="startX">The starting row.</param>
        /// <param name="startY">The starting column.</param>
        /// <returns>The <see cref="PixelBlock" /> representing the selected area.</returns>
        protected PixelBlock FindSingleBlock(int startX, int startY)
        {
            var pixelColors = new List<Color>();

            for (var y = startY; y < startY + this.BlockLength; y++)
            {
                for (var x = startX; x < startX + this.BlockLength; x++)
                {
                    if (CoordinatesAreValid(x, y))
                    {
                        pixelColors.Add(FindPixelColor(x, y));
                    }
                }
            }

            return new PixelBlock(pixelColors);
        }

        private void assignColorToBlock(int startX, int startY, Color color)
        {
            for (var y = startY; y < startY + this.BlockLength && y < Decoder.PixelWidth; y++)
            {
                for (var x = startX; x < startX + this.BlockLength && x < Decoder.PixelHeight; x++)
                {
                    SetPixelColor(x, y, color);
                }
            }
        }

        /// <summary>
        ///     Converts the blocks to black and white.
        /// </summary>
        public virtual void ConvertBlocksToBlackAndWhite()
        {
            for (var x = 0; x < Decoder.PixelHeight; x += this.BlockLength)
            {
                for (var y = 0; y < Decoder.PixelWidth; y += this.BlockLength)
                {
                    var currentBlock = this.FindSingleBlock(x, y);
                    var color = currentBlock.CalculateAverageColor();
                    var average = (color.R + color.B + color.G) / NumberOfColorValues;
                    if (average > HalfBetweenBlackAndWhite)
                    {
                        this.assignColorToBlock(x, y, Colors.White);
                    }
                    else
                    {
                        this.assignColorToBlock(x, y, Colors.Black);
                    }
                }
            }
        }

        #endregion
    }
}