using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;

namespace GroupGMosaicMaker.Model
{
    public class BlockMosaicMaker : ImageGenerator
    {
        public int BlockLength { get; set; }

        public virtual void GenerateMosaic()
        {
            var blocks = this.FindImageBlocks();

            var counter = 0;
            for (var i = 0; i < Decoder.PixelHeight; i += BlockLength)
            {
                for (var j = 0; j < Decoder.PixelWidth; j += BlockLength)
                {
                    var currentBlock = blocks[counter];
                    var color = this.CalculateAverageColor(currentBlock);

                    this.AssignColorToBlock(i, j, color);
                    counter++;
                }
            }
        }

        protected IList<PixelBlock> FindImageBlocks()
        {
            var blocks = new List<PixelBlock>();

            for (var x = 0; x < Decoder.PixelHeight; x += BlockLength)
            {
                for (var y = 0; y < Decoder.PixelWidth; y += BlockLength)
                {
                    blocks.Add(this.FindSingleBlock(x, y));
                }
            }

            return blocks;
        }

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

        private void AssignColorToBlock(int startX, int startY, Color color)
        {
            for (var y = startY; y < startY + BlockLength && y < Decoder.PixelWidth; y++)
            {
                for (var x = startX; x < startX + BlockLength && x < Decoder.PixelHeight; x++)
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
    }
}