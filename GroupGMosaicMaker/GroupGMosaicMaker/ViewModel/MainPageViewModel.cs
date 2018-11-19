using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using GroupGMosaicMaker.DataTier;
using GroupGMosaicMaker.Model;
using GroupGMosaicMaker.Utilities;

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
        private ImageOperator originalImageOperator;

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

        public MainPageViewModel()
        {
            this.loadCommands();
        }

        #region Methods

        private void loadCommands()
        {
            // Add with commands as necessary
        }

        /// <summary>
        ///     Displays the original image asynchronous.
        /// </summary>
        /// <param name="fileStream">The file stream to write the data to.</param>
        /// <returns>The completed asynchronous operation.</returns>
        public async Task DisplayOriginalImageAsync(IRandomAccessStream fileStream)
        {
            this.originalImageOperator= await ImageOperator.CreateAsync(fileStream);
            var image = await this.originalImageOperator.GenerateModifiedImageAsync();
            this.OriginalImage = image;
        }

        public async Task WriteDataAsync(StorageFile file)
        {
            await ImageWriter.WriteImageAsync(this.originalImageOperator, file);
        }

        #endregion
    }
}