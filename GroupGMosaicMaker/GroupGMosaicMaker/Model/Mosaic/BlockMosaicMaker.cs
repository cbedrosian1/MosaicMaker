using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using GroupGMosaicMaker.Model.Image;

namespace GroupGMosaicMaker.Model.Mosaic
{
    public class BlockMosaicMaker : ImageGenerator
    {
        #region Properties

        public int BlockLength { get; set; }

        #endregion

        #region Methods

        public virtual void GenerateMosaic()
        {
            var blocks = this.FindImageBlocks();

            var counter = 0;
            for (var i = 0; i < Decoder.PixelHeight; i += this.BlockLength)
            {
                for (var j = 0; j < Decoder.PixelWidth; j += this.BlockLength)
                {
                    var currentBlock = blocks[counter];
                    var color = this.CalculateAverageColor(currentBlock);

                    this.AssignColorToBlock(i, j, color);
                    counter++;
                }
            }
        }

        protected IList<IList<Color>> FindImageBlocks()
        {
            var blocks = new List<IList<Color>>();

            for (var x = 0; x < Decoder.PixelHeight; x += this.BlockLength)
            {
                for (var y = 0; y < Decoder.PixelWidth; y += this.BlockLength)
                {
                    blocks.Add(this.FindSingleBlock(x, y));
                }
            }

            return blocks;
        }

        protected virtual IList<Color> FindSingleBlock(int startX, int startY)
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

        private void AssignColorToBlock(int startX, int startY, Color color)
        {
            for (var y = startY; y < startY + this.BlockLength && y < Decoder.PixelWidth; y++)
            {
                for (var x = startX; x < startX + this.BlockLength && x < Decoder.PixelHeight; x++)
                {
                    SetPixelColor(x, y, color);
                }
            }
        }

        protected Color CalculateAverageColor(IList<Color> colors)
        {
            var averageR = (byte) colors.Average(color => color.R);
            var averageG = (byte) colors.Average(color => color.G);
            var averageB = (byte) colors.Average(color => color.B);

            return Color.FromArgb(0, averageR, averageG, averageB);
        }

        #endregion
    }
}