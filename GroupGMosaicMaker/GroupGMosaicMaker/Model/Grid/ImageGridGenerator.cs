﻿using Windows.UI;
using GroupGMosaicMaker.Model.Image;

namespace GroupGMosaicMaker.Model.Grid
{
    /// <summary>
    ///     Responsible for drawing square grids of varying sizes onto the given bitmap image.
    /// </summary>
    /// <seealso cref="ImageGenerator" />
    public class ImageGridGenerator : ImageGenerator
    {
        #region Methods

        /// <summary>
        ///     Draws the grid onto the source image.
        /// </summary>
        /// <param name="length">The length.</param>
        public virtual void DrawGrid(int length)
        {
            this.drawVerticalLines(length);
            this.drawHorizontalLines(length);
        }

        private void drawVerticalLines(int length)
        {
            for (var x = 0; x < Decoder.PixelHeight; x += length)
            {
                for (var y = 0; y < Decoder.PixelWidth; ++y)
                {
                    SetPixelColor(x, y, Colors.White);
                }
            }
        }

        private void drawHorizontalLines(int length)
        {
            for (var x = 0; x < Decoder.PixelHeight; ++x)
            {
                for (var y = 0; y < Decoder.PixelWidth; y += length)
                {
                    SetPixelColor(x, y, Colors.White);
                }
            }
        }

        #endregion
    }
}