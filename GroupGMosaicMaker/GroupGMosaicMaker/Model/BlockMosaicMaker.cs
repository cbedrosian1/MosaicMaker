using System.Collections.Generic;
using Windows.UI;

namespace GroupGMosaicMaker.Model
{
    public class BlockMosaicMaker : ImageGenerator
    {
        #region Methods
        
        public void GenerateBlockMosaic(int blockLength)
        {
            var blocks = this.findImageBlocks(blockLength);

            var counter = 0;
            for (var i = 0; i < Decoder.PixelHeight; i += blockLength)
            {
                for (var j = 0; j < Decoder.PixelWidth; j += blockLength)
                {
                    var currentBlock = blocks[counter];
                    var color = currentBlock.CalculateAverageColor();

                    this.assignAverageColorToBlock(i, j, blockLength, color);
                    counter++;
                }
            }
        }
        
        private IList<PixelBlock> findImageBlocks(int blockLength)
        {
            var blocks = new List<PixelBlock>();

            for (var x = 0; x < Decoder.PixelHeight; x += blockLength)
            {
                for (var y = 0; y < Decoder.PixelWidth; y += blockLength)
                {
                    blocks.Add(this.findSingleBlock(x, y, blockLength));
                }
            }

            return blocks;
        }

        private PixelBlock findSingleBlock(int startX, int startY, int blockLength)
        {
            var pixelColors = new List<Color>();

            for (var y = startY; y < startY + blockLength; y++)
            {
                for (var x = startX; x < startX + blockLength; x++)
                {
                    if (this.CoordinatesAreValid(x, y))
                    {
                        pixelColors.Add(FindPixelColor(x, y));
                    }
                }
            }

            return new PixelBlock(pixelColors);
        }

        private void assignAverageColorToBlock(int startX, int startY, int blockLength, Color color)
        {
            for (var y = startY; y < startY + blockLength && y < Decoder.PixelWidth; y++)
            {
                for (var x = startX; x < startX + blockLength && x < Decoder.PixelHeight; x++)
                {
                    SetPixelColor(x, y, color);
                }
            }
        }

        #endregion
    }
}