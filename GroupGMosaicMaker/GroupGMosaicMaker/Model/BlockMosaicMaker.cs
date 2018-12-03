using System.Collections.Generic;
using Windows.UI;

namespace GroupGMosaicMaker.Model
{
    public class BlockMosaicMaker : MosaicMaker
    {
        #region Methods

        protected override PixelBlock FindSingleBlock(int startX, int startY)
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

        #endregion
    }
}