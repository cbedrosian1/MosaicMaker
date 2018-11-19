using Windows.Storage.Streams;

namespace GroupGMosaicMaker.Model
{
    /// <summary>
    ///     Responsible for calculating and creating block mosaics for the given bitmap image.
    /// </summary>
    /// <seealso cref="GroupGMosaicMaker.Model.ImageOperator" />
    public class BlockMosaicMaker : ImageOperator
    {
        #region Constructors

        protected BlockMosaicMaker(IRandomAccessStream imageSource)
        {
        }

        #endregion

        // TODO Add functionality to generate a block mosaic. We can already load an image and return the modified version from the base class.
    }
}