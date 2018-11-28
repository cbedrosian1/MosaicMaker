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

        private IRandomAccessStream imageSource;

        private WriteableBitmap originalImage;
        private readonly ImageOperator originalImageOperator;

        private WriteableBitmap gridImage;
        private readonly ImageGridMaker gridImageOperator;

        private WriteableBitmap mosaicImage;
        private readonly BlockMosaicMaker blockMosaicMaker;

        private WriteableBitmap displayedImage;

        private int gridSize;
        private bool isGridToggled;

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
                if (this.isGridToggled)
                {
                    this.DisplayedImage = this.GridImage;
                }
                
            }
        }

        /// <summary>
        /// Gets or sets the display image.
        /// </summary>
        /// <value>
        /// The display image.
        /// </value>
        public WriteableBitmap DisplayedImage
        {
            get => this.displayedImage;

            set
            {
                this.displayedImage = value;
                this.OnPropertyChanged();
            }

        }


        /// <summary>
        /// Gets or sets the mosaic image.
        /// </summary>
        /// <value>
        /// The mosaic image.
        /// </value>
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
                this.createGridImageAsync(this.imageSource);    
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
        /// Gets or sets the image source.
        /// </summary>
        /// <value>
        /// The image source.
        /// </value>
        public WriteableBitmap ImageSource { get; set; }

        

        public bool IsGridToggled
        {
            get => this.isGridToggled;
            set
            {
                if (value != this.isGridToggled)
                {
                    this.isGridToggled = value;
                }
                
            }
        }


        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPageViewModel"/> class.
        /// </summary>
        public MainPageViewModel()
        {
            this.originalImageOperator = new ImageMaker();
            this.gridImageOperator = new ImageGridMaker();
            this.blockMosaicMaker = new BlockMosaicMaker();

            this.gridSize = 10;
            this.canSaveImage = false;

            this.loadCommands();
        }

        #region Methods

        private void loadCommands()
        {
            this.GenerateMosaicCommand = new RelayCommand(this.generateMosaic, this.canGenerateMosaic);
        }

        private bool canGenerateMosaic(object obj)
        {
            return this.OriginalImage != null;
        }

        private async void generateMosaic(object obj)
        {
            this.blockMosaicMaker.GenerateBlockMosaic(this.GridSize);

            this.MosaicImage = await this.blockMosaicMaker.GenerateImageAsync();
        }

        /// <summary>
        ///     Displays the original image asynchronous.
        /// </summary>
        /// <param name="imageSource">The source of the image data.</param>
        /// <returns>The completed asynchronous operation.</returns>
        public async Task CreateImages(IRandomAccessStream imageSource)
        {
            this.imageSource = imageSource.CloneStream();

            await this.blockMosaicMaker.SetSourceAsync(imageSource);
            await this.createOriginalImageAsync(imageSource);
            this.createGridImageAsync(imageSource);

            if (this.IsGridToggled)
            {
                this.DisplayedImage = this.gridImage;
            }
            else
            {
                this.DisplayedImage = this.originalImage;
            }
            
        }

        private async Task createOriginalImageAsync(IRandomAccessStream imageSource)
        {
            await this.originalImageOperator.SetSourceAsync(imageSource);
            this.OriginalImage = await this.originalImageOperator.GenerateImageAsync();
            this.CanSaveImage = true;
        }

        private async void createGridImageAsync(IRandomAccessStream imageSource)
        {
            await this.gridImageOperator.SetSourceAsync(imageSource);
            this.gridImageOperator.DrawGrid(this.GridSize);
            this.GridImage = await this.gridImageOperator.GenerateImageAsync();
        }

        /// <summary>
        /// Writes the data asynchronous.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public async Task WriteDataAsync(StorageFile file)
        {
            await ImageWriter.WriteImageAsync(this.blockMosaicMaker, file);
        }


        #endregion
    }
}