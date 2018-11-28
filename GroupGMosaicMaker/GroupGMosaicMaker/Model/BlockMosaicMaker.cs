using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI;

namespace GroupGMosaicMaker.Model
{
    public class BlockMosaicMaker : ImageMaker
    {
        public void GenerateBlockMosaic(int blockLength)
        {
            var blocks = this.findImageBlocks(blockLength);

            var counter = 0;
            for (int i = 0; i < Decoder.PixelHeight; i += blockLength)
            {
                for (int j = 0; j < Decoder.PixelWidth; j += blockLength)
                {
                    this.assignAverageColorToBlock(i, j, blockLength, blocks[counter].CalculateAverageColor());
                    counter++;
                }
            }
        }

        private IList<ImageBlock> findImageBlocks(int blockLength)
        {
            var blocks = new List<ImageBlock>();

            for (int i = 0; i < Decoder.PixelHeight; i += blockLength)
            {
                for (int j = 0; j < Decoder.PixelWidth; j += blockLength)
                {
                    blocks.Add(this.findSingleBlock(i, j, blockLength));
                }
            }

            return blocks;
        }

        private ImageBlock findSingleBlock(int startX, int startY, int blockLength)
        {
            var pixelColors = new List<Color>();

            for (int x = startX; x < startX + blockLength; x++)
            {
                for (int y = startY; y < startY + blockLength; y++)
                {
                    pixelColors.Add(GetPixelBgra8(x, y));
                }
            }

            return new ImageBlock(pixelColors);
        }

        private void assignAverageColorToBlock(int startX, int startY, int blockLength, Color color)
        {
            for (int x = startX; x < startX + blockLength; x++)
            {
                for (int y = startY; y < startY + blockLength; y++)
                {
                    this.SetPixelBgra8(x, y, color);
                }
            }
        }
    }
}