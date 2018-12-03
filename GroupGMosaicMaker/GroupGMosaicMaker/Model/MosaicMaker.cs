using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI;

namespace GroupGMosaicMaker.Model
{
    public abstract class MosaicMaker : ImageGenerator
    {
        public int BlockLength { get; set; }

        public void GenerateMosaic()
        {
            var blocks = this.findImageBlocks();

            var counter = 0;
            for (var i = 0; i < Decoder.PixelHeight; i += BlockLength)
            {
                for (var j = 0; j < Decoder.PixelWidth; j += BlockLength)
                {
                    var currentBlock = blocks[counter];
                    var color = currentBlock.CalculateAverageColor();

                    this.assignColorToBlock(i, j, color);
                    counter++;
                }
            }
        }

        private IList<PixelBlock> findImageBlocks()
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

        protected abstract PixelBlock FindSingleBlock(int x, int y);

        private void assignColorToBlock(int startX, int startY, Color color)
        {
            for (var y = startY; y < startY + BlockLength && y < Decoder.PixelWidth; y++)
            {
                for (var x = startX; x < startX + BlockLength && x < Decoder.PixelHeight; x++)
                {
                    SetPixelColor(x, y, color);
                }
            }
        }
    }
}