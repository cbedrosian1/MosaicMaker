using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.UI.Xaml.Media.Imaging;
using GroupGMosaicMaker.Model;

namespace GroupGMosaicMaker.ViewModel
{
    /// <summary>
    ///     ViewModel for the main page.
    /// </summary>
    /// <seealso cref="GroupGMosaicMaker.ViewModel.ViewModelGeneric" />
    public class MainPageViewModel : ViewModelGeneric
    {
        #region Data members

        private WriteableBitmap originalImage;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the original image.
        /// </summary>
        /// <value>
        ///     The original image.
        /// </value>
        public WriteableBitmap OriginalImage
        {
            get => this.originalImage;
            set
            {
                this.originalImage = value;
                OnPropertyChanged();
            }
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Displays the original image asynchronous.
        /// </summary>
        /// <param name="decoder">The decoder.</param>
        /// <returns>The completed asynchronous operation.</returns>
        public async Task DisplayOriginalImageAsync(BitmapDecoder decoder)
        {
            var imageOperator = await ImageOperator.CreateAsync(decoder);
            var image = await imageOperator.GenerateModifiedImageAsync();

            this.OriginalImage = image;
        }

        #endregion
    }
}