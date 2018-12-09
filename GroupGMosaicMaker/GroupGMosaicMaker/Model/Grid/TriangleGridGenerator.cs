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

            for (var i = 0; i < Decoder.PixelHeight; i += length)
            {
                for (var j = 0; j < Decoder.PixelWidth; j += length)
                {
                    this.drawDiagonal(i, j, length);
                }
            }
        }

        private void drawDiagonal(int startX, int startY, int blockLength)
        {
            var x = startX;
            for (var y = startY; y < startY + blockLength && y < Decoder.PixelWidth; ++y)
            {
                SetPixelColor(x, y, Colors.White);
                ++x;
            }
        }

        #endregion
    }
}