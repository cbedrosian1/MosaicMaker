using Windows.UI;
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

        public virtual void DrawGrid(int length)
        {
            for (var i = 0; i < Decoder.PixelHeight; i += length)
            {
                for (var j = 0; j < Decoder.PixelWidth; j++)
                {
                    SetPixelColor(i, j, Colors.White);
                }
            }

            for (var i = 0; i < Decoder.PixelHeight; i++)
            {
                for (var j = 0; j < Decoder.PixelWidth; j += length)
                {
                    SetPixelColor(i, j, Colors.White);
                }
            }
        }

        #endregion
    }
}