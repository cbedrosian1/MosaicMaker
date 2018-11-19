namespace GroupGMosaicMaker.Model
{
    // TODO Better class name. Needs to be distinguished between this and DataTier::ImageWriter
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

        protected ImageGridWriter(int length)
        {
            this.length = length;
        }

        #endregion

        // TODO Add functionality to draw grid. We can already load an image and return the modified version from the base class.
    }
}