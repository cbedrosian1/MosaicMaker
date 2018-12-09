using System.Collections.Generic;
using Windows.UI;

namespace GroupGMosaicMaker.Model.Mosaic
{
    /// <summary>
    ///     Responsible for drawing equilateral right triangular block mosaics, with the side length of the triangle defined by
    ///     the block length.
    /// </summary>
    /// <seealso cref="GroupGMosaicMaker.Model.Mosaic.MosaicMaker" />
    public class TriangleMosaicMaker : MosaicMaker
    {
        #region Methods
        private const int LowerTriangularStartingOffset = 0;
        private const int UpperTriangularStartingOffset = 1;

        /// <summary>
        ///     Generates the (x, y)'th block of the mosaic. Override this method to change the functionality of how each block is
        ///     generated.
        /// </summary>
        /// <param name="x">The row.</param>
        /// <param name="y">The column.</param>
        protected override void GenerateMosaicBlock(int x, int y)
        {
            this.generateLowerTriangle(x, y);
            this.generateUpperTriangle(x, y);
        }

        private void generateLowerTriangle(int x, int y)
        {
            var currentLowerTriangularBlock = this.findLowerTriangularBlock(x, y);
            var currentLowerColor = currentLowerTriangularBlock.CalculateAverageColor();

            this.assignColorToLowerTriangularBlock(x, y, currentLowerColor);
        }

        private void generateUpperTriangle(int x, int y)
        {
            var currentUpperTriangularBlock = this.findUpperTriangularBlock(x, y);
            var currentUpperColor = currentUpperTriangularBlock.CalculateAverageColor();

            this.assignColorToUpperTriangularBlock(x, y, currentUpperColor);
        }

        private PixelBlock findUpperTriangularBlock(int startX, int startY)
        {
            var pixelColors = new List<Color>();

            var pixelsInRows = UpperTriangularStartingOffset;
            for (var y = startY; y < startY + BlockLength; ++y)
            {
                for (var x = startX; x < startX + pixelsInRows; ++x)
                {
                    var color = FindPixelColor(x, y);
                    pixelColors.Add(color);
                }

                ++pixelsInRows;
            }

            return new PixelBlock(pixelColors);
        }

        private PixelBlock findLowerTriangularBlock(int startX, int startY)
        {
            var pixelColors = new List<Color>();

            var pixelsInRows = LowerTriangularStartingOffset;
            for (var x = startX; x < startX + BlockLength; ++x)
            {
                for (var y = startY; y < startY + pixelsInRows; ++y)
                {
                    var color = FindPixelColor(x, y);
                    pixelColors.Add(color);
                }

                ++pixelsInRows;
            }

            return new PixelBlock(pixelColors);
        }

        private void assignColorToUpperTriangularBlock(int startX, int startY, Color color)
        {
            var pixelsInRows = UpperTriangularStartingOffset;
            for (var y = startY; y < startY + BlockLength && y < Decoder.PixelWidth; ++y)
            {
                for (var x = startX; x < startX + pixelsInRows; ++x)
                {
                    SetPixelColor(x, y, color);
                }

                ++pixelsInRows;
            }
        }

        private void assignColorToLowerTriangularBlock(int startX, int startY, Color color)
        {
            var pixelsInRows = LowerTriangularStartingOffset;

            for (var x = startX; x < startX + BlockLength; ++x)   
            {
                for (var y = startY; y < startY + pixelsInRows && y < Decoder.PixelWidth; ++y)
                {
                    SetPixelColor(x, y, color);
                }

                ++pixelsInRows;
            }
        }

        /// <summary>
        ///     Converts the mosaic to black and white.
        /// </summary>
        public override void ConvertToBlackAndWhite()
        {
            for (var y = 0; y < Decoder.PixelHeight; y += BlockLength)
            {
                for (var x = 0; x < Decoder.PixelWidth; x += BlockLength)
                {
                    this.convertLowerTriangularBlockToBlackAndWhite(x, y);
                    this.convertUpperTriangularBlockToBlackAndWhite(x, y);
                }
            }
        }

        private void convertLowerTriangularBlockToBlackAndWhite(int startX, int startY)
        {
            var pixelsInRows = LowerTriangularStartingOffset;
            for (var x = startX; x < startX + BlockLength; ++x)
            {
                for (var y = startY; y < startY + pixelsInRows; ++y)
                {
                    ConvertPixelToBlackAndWhite(x, y);
                }

                ++pixelsInRows;
            }
        }

        private void convertUpperTriangularBlockToBlackAndWhite(int startX, int startY)
        {
            var pixelsInRows = UpperTriangularStartingOffset;
            for (var y = startY; y < startY + BlockLength; ++y)
            {
                for (var x = startX; x < startX + pixelsInRows; ++x)
                {
                    ConvertPixelToBlackAndWhite(x, y);
                }

                ++pixelsInRows;
            }
        }

        #endregion
    }
}