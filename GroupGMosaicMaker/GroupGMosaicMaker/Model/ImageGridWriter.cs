using Windows.Graphics.Imaging;

namespace GroupGMosaicMaker.Model
{
    /// <summary>
    ///     Responsible for drawing square grids of varying sizes onto the given bitmap image
    /// </summary>
    /// <seealso cref="GroupGMosaicMaker.Model.ImageOperator" />
    public class ImageGridWriter : ImageOperator
    {
        #region Data members

        private int length;

        #endregion

        #region Constructors

        protected ImageGridWriter(BitmapDecoder decoder, int length) : base(decoder)
        {
            this.length = length;
        }

        #endregion

        // TODO Add functionality to draw grid. We can already load an image and return the modified version from the base class.
    }
}