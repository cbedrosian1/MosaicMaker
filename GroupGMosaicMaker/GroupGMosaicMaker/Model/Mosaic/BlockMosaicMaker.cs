using System.Collections.Generic;
using Windows.UI;
using GroupGMosaicMaker.Extensions;
using GroupGMosaicMaker.Model.Image;

namespace GroupGMosaicMaker.Model.Mosaic
{
    /// <summary>
    ///     Responsible for generating solid block mosaics from the image source, of varying block sizes.
    /// </summary>
    /// <seealso cref="GroupGMosaicMaker.Model.Image.ImageGenerator" />
    public class BlockMosaicMaker : MosaicMaker
    {
        #region Methods


        /// <summary>
        ///     Generates the (x, y)'th block of the mosaic. Override this method to change the functionality of how each block is
        ///     generated.
        /// </summary>
        /// <param name="x">The row.</param>
        /// <param name="y">The column.</param>
        protected override void GenerateMosaicBlock(int x, int y)
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
        protected virtual PixelBlock FindSingleBlock(int startX, int startY)
        {
            var pixelColors = new List<Color>();

            for (var y = startY; y < startY + BlockLength; y++)
            {
                for (var x = startX; x < startX + BlockLength; x++)
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
        ///     Converts the mosaic to black and white.
        /// </summary>
        public override void ConvertToBlackAndWhite()
        {
            for (var x = 0; x < Decoder.PixelHeight; x += this.BlockLength)
            {
                for (var y = 0; y < Decoder.PixelWidth; y += this.BlockLength)
                {
                    this.convertBlockToBlackAndWhite(x, y);
                }
            }
        }

        private void convertBlockToBlackAndWhite(int startX, int startY)
        {
            for (var y = startY; y < startY + this.BlockLength && y < Decoder.PixelWidth; y++)
            {
                for (var x = startX; x < startX + this.BlockLength && x < Decoder.PixelHeight; x++)
                {
                    this.ConvertPixelToBlackAndWhite(x, y);
                }
            }
        }

        #endregion
    }
}