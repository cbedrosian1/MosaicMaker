using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using GroupGMosaicMaker.DataTier;
using GroupGMosaicMaker.Model;
using GroupGMosaicMaker.Utilities;
using System;

namespace GroupGMosaicMaker.ViewModel
{
    /// <summary>
    ///     ViewModel for the main page.
    /// </summary>
    /// <seealso cref="GroupGMosaicMaker.ViewModel.ViewModelGeneric" />
    public class MainPageViewModel : ViewModelGeneric
    {
        #region Data members

        private bool canSaveImage;

        private WriteableBitmap originalImage;
        private ImageOperator originalImageOperator;

        private WriteableBitmap gridImage;
        private ImageGridMaker gridImageOperator;
        private WriteableBitmap mosaicImage;
        private WriteableBitmap displayImage;

        private int gridSize;

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
                this.GenerateMosaicCommand.OnCanExecuteChanged();
                this.GenerateGridImage.OnCanExecuteChanged();
            }
        }

        /// <summary>
        /// Gets or sets the grid image.
        /// </summary>
        /// <value>
        /// The grid image.
        /// </value>
        public WriteableBitmap GridImage
        {
            get => this.gridImage;
            set
            {
                this.gridImage = value;
                OnPropertyChanged();
            }
        }

        public WriteableBitmap DisplayImage
        {
            get => this.displayImage;

            set
            {
                this.displayImage = value;
                this.OnPropertyChanged();
            }

        }


        public WriteableBitmap MosaicImage
        {
            get => this.mosaicImage;
            set
            {
                this.mosaicImage = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance can save image.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance can save image; otherwise, <c>false</c>.
        /// </value>
        public bool CanSaveImage
        {
            get => this.canSaveImage;
            set
            {
                this.canSaveImage = value;
                OnPropertyChanged();
            }
        }



        /// <summary>
        /// Gets or sets the size of the grid for the mosaic.
        /// </summary>
        /// <value>
        /// The size of the grid for the mosaic.
        /// </value>
        public int GridSize
        {
            get => this.gridSize;
            set
            {
                this.gridSize = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the generate mosaic command.
        /// </summary>
        /// <value>
        /// The generate mosaic command.
        /// </value>
        public RelayCommand GenerateMosaicCommand { get; set; }

        /// <summary>
        /// Gets or sets the generate grid image.
        /// </summary>
        /// <value>
        /// The generate grid image.
        /// </value>
        public RelayCommand GenerateGridImage { get; set; }

        /// <summary>
        /// Gets or sets the image source.
        /// </summary>
        /// <value>
        /// The image source.
        /// </value>
        public WriteableBitmap ImageSource { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPageViewModel"/> class.
        /// </summary>
        public MainPageViewModel()
        {
            this.gridSize = 10;
            this.canSaveImage = false;

            this.loadCommands();
        }

        #region Methods

        private void loadCommands()
        {
            this.GenerateMosaicCommand = new RelayCommand(this.generateMosaic, this.canGenerateMosaic);
            this.GenerateGridImage = new RelayCommand(this.generateGrid, this.canGenerateGrid);
        }

        private bool canGenerateGrid(object obj)
        {
            return this.OriginalImage != null;
        }

        private async void generateGrid(object obj)
        {
            this.DisplayImage = this.gridImage;
        }

        private bool canGenerateMosaic(object obj)
        {
            return this.OriginalImage != null;
        }

        private void generateMosaic(object obj)
        {
            throw new NotImplementedException(); //TODO
        }

        /// <summary>
        ///     Displays the original image asynchronous.
        /// </summary>
        /// <param name="imageSource">The source of the image data.</param>
        /// <returns>The completed asynchronous operation.</returns>
        public async Task DisplayImages(IRandomAccessStream imageSource)
        {
            
            await this.displayOriginalImageAsync(imageSource);
            await this.displayGridImageAsync(imageSource);
            this.DisplayImage = OriginalImage;
           

        }

        private async Task displayOriginalImageAsync(IRandomAccessStream imageSource)
        {
            this.originalImageOperator = await ImageOperator.CreateAsync(imageSource);
            this.OriginalImage = await this.originalImageOperator.GenerateModifiedImageAsync();

            this.CanSaveImage = true;
        }

        private async Task displayGridImageAsync(IRandomAccessStream imageSource)
        {
            this.gridImageOperator = await ImageGridMaker.CreateAsync(imageSource);
            this.gridImageOperator.DrawGrid(this.GridSize);
            this.GridImage = await this.gridImageOperator.GenerateModifiedImageAsync();
        }

        /// <summary>
        /// Writes the data asynchronous.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public async Task WriteDataAsync(StorageFile file)
        {
            await ImageWriter.WriteImageAsync(this.gridImageOperator, file);
        }

        #endregion
    }
}