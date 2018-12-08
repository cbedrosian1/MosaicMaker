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

            var pixelsInRows = 1;
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

            var pixelsInRows = 0;
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
            var pixelsInRows = 1;
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
            var pixelsInRows = 0;
            for (var x = startX; x < startX + BlockLength; ++x)
            {
                for (var y = startY; y < startY + pixelsInRows && y < Decoder.PixelWidth; ++y)
                {
                    SetPixelColor(x, y, color);
                }

                ++pixelsInRows;
            }
        }

        #endregion

        public override void ConvertToBlackAndWhite()
        {
            for (var x = 0; x < Decoder.PixelHeight; x += BlockLength)
            {
                for (var y = 0; y < Decoder.PixelWidth; y += BlockLength)
                {
                    this.convertLowerTriangularBlockToBlackAndWhite(x, y);
                    this.convertUpperTriangularBlockToBlackAndWhite(x, y);
                }
            }
        }

        private void convertLowerTriangularBlockToBlackAndWhite(int startX, int startY)
        {
            var pixelsInRows = 0;
            for (var x = startX; x < startX + BlockLength; ++x)
            {
                for (var y = startY; y < startY + pixelsInRows; ++y)
                {
                    this.ConvertPixelToBlackAndWhite(x, y);
                }

                ++pixelsInRows;
            }
        }

        private void convertUpperTriangularBlockToBlackAndWhite(int startX, int startY)
        {
            var pixelsInRows = 1;
            for (var y = startY; y < startY + BlockLength; ++y)
            {
                for (var x = startX; x < startX + pixelsInRows; ++x)
                {
                    this.ConvertPixelToBlackAndWhite(x, y);
                }

                ++pixelsInRows;
            }
        }
    }
}