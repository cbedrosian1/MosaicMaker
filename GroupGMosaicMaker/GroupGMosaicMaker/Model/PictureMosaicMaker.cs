using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Windows.Devices.Perception.Provider;
using Windows.UI;

namespace GroupGMosaicMaker.Model
{
    public class PictureMosaicMaker : BlockMosaicMaker
    {
        private IList<PixelBlock> sourceBlocks;
        private IList<PaletteImageGenerator> palette;

        public override void GenerateMosaic()
        {
            var blocks = this.FindImageBlocks();

            var counter = 0;
            for (var i = 0; i < Decoder.PixelHeight; i += BlockLength)
            {
                for (var j = 0; j < Decoder.PixelWidth; j += BlockLength)
                {
                    var currentBlock = blocks[counter];
                    var currentBlockColor = this.CalculateAverageColor(currentBlock);
                    var closestImage = this.findClosestPaletteImage(currentBlockColor);

                    this.assignPaletteImageToBlock(i, j, closestImage);
                    counter++;
                }
            }
        }

        public IList<PaletteImageGenerator> Palette
        {
            get => this.palette;
            set
            {
                this.palette = value;
                this.assignAverageColorsToPallete();
            }
        }

        private void assignAverageColorsToPallete()
        {
            foreach (var paletteImage in this.Palette)
            {
                paletteImage.AverageColor = CalculateAverageColor(paletteImage.Pixels);
            }
        }

        private PaletteImageGenerator findClosestPaletteImage(Color color)
        {
            var colorDifferences = new Dictionary<PaletteImageGenerator, int>();
            foreach (var paletteImage in this.Palette)
            {
                var currentColor = paletteImage.AverageColor;
                var rDifference = color.R - currentColor.R;
                var gDifference = color.G - currentColor.G;
                var bDifference = color.B - currentColor.B;

                var sumOfDifferences = Math.Abs(rDifference) + Math.Abs(gDifference) + Math.Abs(bDifference);
                colorDifferences.Add(paletteImage, sumOfDifferences);
            }

            var keyAndValue = colorDifferences.OrderBy(kvp => kvp.Value).First();
            return keyAndValue.Key;
        }

        private void assignPaletteImageToBlock(int startX, int startY, PaletteImageGenerator paletteImage)
        {
            for (var y = startY; y < startY + BlockLength && y < Decoder.PixelWidth; y++)
            {
                for (var x = startX; x < startX + BlockLength && x < Decoder.PixelHeight; x++)
                {
                    var currentPaletteColor = paletteImage.FindPixelColor(x - startX, y - startY);
                    SetPixelColor(x, y, currentPaletteColor);
                }
            }
        }
    }
}