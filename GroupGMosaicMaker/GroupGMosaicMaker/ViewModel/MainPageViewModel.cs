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
        private readonly ImageGenerator originalImageGenerator;

        private WriteableBitmap gridImage;
        private readonly ImageGridGenerator gridImageOperator;

        private WriteableBitmap mosaicImage;
        private readonly BlockMosaicMaker blockMosaicMaker;

        private WriteableBitmap displayedImage;

        private WriteableBitmap triangleGridImage;
        private readonly TriangleGridGenerator triangleGridImageOperator;

        private int gridSize;
        private bool isSquareGridSelected;

        private const int DefaultGridSize = 10;

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
                this.GenerateBlockMosaicCommand.OnCanExecuteChanged();
                this.GeneratePictureMosaicCommand.OnCanExecuteChanged(); //TODO will probably need to add this to the collection property
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
                this.OnPropertyChanged();
                if (this.IsGridToggled && this.isSquareGridSelected)
                {
                    this.DisplayedImage = this.GridImage;
                }
                
            }
        }



        /// <summary>
        /// Gets or sets the triangle grid image.
        /// </summary>
        /// <value>
        /// The triangle grid image.
        /// </value>
        public WriteableBitmap TriangleGridImage
        {
            get => triangleGridImage;
            set
            {
                triangleGridImage = value;
                this.OnPropertyChanged();
                if (this.IsGridToggled && !this.isSquareGridSelected)
                {
                    this.DisplayedImage = this.TriangleGridImage;
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
                this.CanSaveImage = true;
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
                if (this.imageSource != null)
                {
                    this.createGridImageAsync(this.imageSource);
                    this.createTriangleGridImageAsync(this.imageSource);
                }
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the generate block mosaic command.
        /// </summary>
        /// <value>
        /// The generate block mosaic command.
        /// </value>
        public RelayCommand GenerateBlockMosaicCommand { get; set; }

        /// <summary>
        /// Gets or sets the generate picture mosaic command.
        /// </summary>
        /// <value>
        /// The generate picture mosaic command.
        /// </value>
        public RelayCommand GeneratePictureMosaicCommand { get; set; }

        /// <summary>
        /// Gets or sets the image source.
        /// </summary>
        /// <value>
        /// The image source.
        /// </value>
        public WriteableBitmap ImageSource { get; set; }



        /// <summary>
        /// Gets or sets a value indicating whether the grid is toggled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the grid is toggled; otherwise, <c>false</c>.
        /// </value>
        public bool IsGridToggled { get; set; }



        /// <summary>
        /// Gets or sets a value indicating whether square grid is selected.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this square is grid selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSquareGridSelected
        {
            get =>isSquareGridSelected;

            set
            {
                isSquareGridSelected = value; 
                this.OnPropertyChanged();
                this.createGridImageAsync(this.imageSource);
                this.createTriangleGridImageAsync(this.imageSource);
                this.GeneratePictureMosaicCommand.OnCanExecuteChanged();
            }
        }



        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPageViewModel"/> class.
        /// </summary>
        public MainPageViewModel()
        {
            this.originalImageGenerator = new ImageGenerator();
            this.gridImageOperator = new ImageGridGenerator();
            this.blockMosaicMaker = new BlockMosaicMaker();
            this.triangleGridImageOperator = new TriangleGridGenerator();

            this.gridSize = DefaultGridSize;
            this.canSaveImage = false;
            this.isSquareGridSelected = true;

            this.loadCommands();
        }

        #region Methods

        private void loadCommands()
        {
            this.GenerateBlockMosaicCommand = new RelayCommand(this.generateMosaic, this.canGenerateBlockMosaic);
            this.GeneratePictureMosaicCommand = new RelayCommand(this.generateMosaic, this.canGeneratePictureMosaic);
        }

        private bool canGenerateBlockMosaic(object obj)
        {
            return this.OriginalImage != null;
        }

        private bool canGeneratePictureMosaic(object obj)
        {
            return this.originalImage != null && !this.isSquareGridSelected;
            //TODO When pallete != null 
        }

        private async void generateMosaic(object obj)
        {
            //TODO might have to create separate command for picture mosaic
            await this.blockMosaicMaker.SetSourceAsync(this.imageSource);
            this.blockMosaicMaker.BlockLength = this.GridSize;
            this.blockMosaicMaker.GenerateMosaic();

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
            
            await this.createOriginalImageAsync(imageSource);
            this.createGridImageAsync(imageSource);
            this.createTriangleGridImageAsync(imageSource);
            
            this.displayImageOnCreation();
            
        }

        private void displayImageOnCreation()
        {
            if (this.IsGridToggled)
            {
                if (this.isSquareGridSelected)
                {
                    this.DisplayedImage = this.gridImage;
                }
                else
                {
                    this.DisplayedImage = this.triangleGridImage;
                }
                
            }
            else
            {
                this.DisplayedImage = this.originalImage;
            }
        }

        private async Task createOriginalImageAsync(IRandomAccessStream imageSource)
        {
            await this.originalImageGenerator.SetSourceAsync(imageSource);
            this.OriginalImage = await this.originalImageGenerator.GenerateImageAsync();
            
        }

        private async void createGridImageAsync(IRandomAccessStream imageSource)
        {
            await this.gridImageOperator.SetSourceAsync(imageSource);
            this.gridImageOperator.DrawGrid(this.GridSize);
            this.GridImage = await this.gridImageOperator.GenerateImageAsync();
        }

        private async void createTriangleGridImageAsync(IRandomAccessStream imageSource)
        {
            await this.triangleGridImageOperator.SetSourceAsync(imageSource);
            this.triangleGridImageOperator.DrawGrid(this.GridSize);
            this.TriangleGridImage = await this.triangleGridImageOperator.GenerateImageAsync();
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