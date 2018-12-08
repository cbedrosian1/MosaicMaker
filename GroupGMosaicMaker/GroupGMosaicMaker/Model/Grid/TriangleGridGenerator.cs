using Windows.UI;

namespace GroupGMosaicMaker.Model.Grid
{
    /// <summary>
    ///     Generates grids in an equilateral triangle pattern.
    /// </summary>
    /// <seealso cref="GroupGMosaicMaker.Model.Grid.ImageGridGenerator" />
    internal class TriangleGridGenerator : ImageGridGenerator
    {
        #region Methods

        /// <summary>
        ///     Draws the grid onto the source image.
        /// </summary>
        /// <param name="length">The length.</param>
        public override void DrawGrid(int length)
        {
            base.DrawGrid(length);

            for (var y = 0; y < Decoder.PixelHeight; y += length)
            {
                for (var x = 0; x < Decoder.PixelWidth; x += length)
                {
                    this.drawDiagonal(x, y, length);
                }
            }
        }

        private void drawDiagonal(int startX, int startY, int blockLength)
        {
            var y = startY;
            for (var x = startX; x < startX + blockLength && x < Decoder.PixelWidth; ++x)
            {
                SetPixelColor(x, y, Colors.White);
                ++y;
            }
        }

        #endregion
    }
}