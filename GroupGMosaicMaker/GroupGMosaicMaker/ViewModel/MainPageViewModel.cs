using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using GroupGMosaicMaker.DataTier;
using GroupGMosaicMaker.Model.Grid;
using GroupGMosaicMaker.Model.Image;
using GroupGMosaicMaker.Model.Mosaic;
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

        private const int DefaultGridSize = 10;

        private bool canSaveImage;
        private bool isUsingSelectedImages;
        private bool isImageSelected;
        private bool isSquareGridSelected;
        private bool isZoomSelected;

        private IRandomAccessStream imageSource;

        private ObservableCollection<PaletteImageGenerator> palette;
        private ObservableCollection<PaletteImageGenerator> selectedPalette;

        private WriteableBitmap originalImage;
        private readonly ImageGenerator originalImageGenerator;

        private WriteableBitmap gridImage;
        private readonly ImageGridGenerator gridImageGenerator;

        private WriteableBitmap mosaicImage;
        private readonly BlockMosaicMaker blockMosaicMaker;
        private readonly TriangleMosaicMaker triangleMosaicMaker;
        private readonly PictureMosaicMaker pictureMosaicMaker;

        private WriteableBitmap displayedSourceImage;

        private WriteableBitmap triangleGridImage;
        private readonly TriangleGridGenerator triangleGridImageGenerator;

        private int gridSize;

        private PaletteImageGenerator selectedImage;

        private int paletteCount;

        private WriteableBitmap blackAndWhiteMosaic;
        private WriteableBitmap displayedMosaicImage;

        private bool isUseImagesEvenlyChecked;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the palette count.
        /// </summary>
        /// <value>
        ///     The palette count.
        /// </value>
        public int PaletteCount
        {
            get => this.paletteCount;
            set
            {
                this.paletteCount = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether there is an image selected.
        /// </summary>
        /// <value>
        ///     <c>true</c> if there is an image selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsImageSelected
        {
            get => this.isImageSelected;
            set
            {
                this.isImageSelected = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether is using selected images for picture mosaic.
        /// </summary>
        /// <value>
        ///     <c>true</c> if is using selected images; otherwise, <c>false</c>.
        /// </value>
        public bool IsUsingSelectedImages
        {
            get => this.isUsingSelectedImages;
            set
            {
                this.isUsingSelectedImages = value;
                OnPropertyChanged();
                if (!this.IsUsingSelectedImages)
                {
                    this.SelectedPalette = this.Palette;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the selected palette.
        /// </summary>
        /// <value>
        ///     The selected palette.
        /// </value>
        public ObservableCollection<PaletteImageGenerator> SelectedPalette
        {
            get => this.selectedPalette;
            set
            {
                this.selectedPalette = value;
                OnPropertyChanged();
                this.GeneratePictureMosaicCommand.OnCanExecuteChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the palette.
        /// </summary>
        /// <value>
        ///     The palette.
        /// </value>
        public ObservableCollection<PaletteImageGenerator> Palette
        {
            get => this.palette;
            set
            {
                this.palette = value;
                OnPropertyChanged();
                this.PaletteCount = this.palette.Count;
                this.ClearPaletteImagesCommand.OnCanExecuteChanged();
            }
        }

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
                this.GeneratePictureMosaicCommand
                    .OnCanExecuteChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the grid image.
        /// </summary>
        /// <value>
        ///     The grid image.
        /// </value>
        public WriteableBitmap GridImage
        {
            get => this.gridImage;
            set
            {
                this.gridImage = value;
                OnPropertyChanged();
                if (this.IsGridToggled && this.isSquareGridSelected)
                {
                    this.DisplayedSourceImage = this.GridImage;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the triangle grid image.
        /// </summary>
        /// <value>
        ///     The triangle grid image.
        /// </value>
        public WriteableBitmap TriangleGridImage
        {
            get => this.triangleGridImage;
            set
            {
                this.triangleGridImage = value;
                OnPropertyChanged();
                if (this.IsGridToggled && !this.isSquareGridSelected)
                {
                    this.DisplayedSourceImage = this.TriangleGridImage;
                }
            }
        }

        /// <summary>
        ///     Gets or sets the display image.
        /// </summary>
        /// <value>
        ///     The display image.
        /// </value>
        public WriteableBitmap DisplayedSourceImage
        {
            get => this.displayedSourceImage;

            set
            {
                this.displayedSourceImage = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the mosaic image.
        /// </summary>
        /// <value>
        ///     The mosaic image.
        /// </value>
        public WriteableBitmap MosaicImage
        {
            get => this.mosaicImage;
            set
            {
                this.mosaicImage = value;
                this.CanSaveImage = value != null;

                OnPropertyChanged();
            }
        }

        public WriteableBitmap DisplayedMosaicImage
        {
            get => this.displayedMosaicImage;
            set
            {
                this.displayedMosaicImage = value;
                OnPropertyChanged();
            }
        }

        public WriteableBitmap BlackAndWhiteMosaic
        {
            get => this.blackAndWhiteMosaic;
            set
            {
                this.blackAndWhiteMosaic = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance can save image.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance can save image; otherwise, <c>false</c>.
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
        ///     Gets or sets the size of the grid for the mosaic.
        /// </summary>
        /// <value>
        ///     The size of the grid for the mosaic.
        /// </value>
        public int GridSize
        {
            get => this.gridSize;
            set
            {
                this.gridSize = value;

                this.blockMosaicMaker.BlockLength = this.gridSize;
                this.triangleMosaicMaker.BlockLength = this.gridSize;
                this.pictureMosaicMaker.BlockLength = this.gridSize;

                if (this.imageSource != null)
                {
                    this.createGridImageAsync();
                    this.createTriangleGridImageAsync();
                }

                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the generate block mosaic command.
        /// </summary>
        /// <value>
        ///     The generate block mosaic command.
        /// </value>
        public RelayCommand GenerateBlockMosaicCommand { get; set; }

        /// <summary>
        ///     Gets or sets the generate picture mosaic command.
        /// </summary>
        /// <value>
        ///     The generate picture mosaic command.
        /// </value>
        public RelayCommand GeneratePictureMosaicCommand { get; set; }

        /// <summary>
        ///     Gets or sets the clear palette images command.
        /// </summary>
        /// <value>
        ///     The clear palette images command.
        /// </value>
        public RelayCommand ClearPaletteImagesCommand { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the grid is toggled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the grid is toggled; otherwise, <c>false</c>.
        /// </value>
        public bool IsGridToggled { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether black and white is toggled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if black white is toggled; otherwise, <c>false</c>.
        /// </value>
        public bool IsBlackWhiteToggled { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether use images evenly is checked.
        /// </summary>
        /// <value>
        ///     <c>true</c> if use images evenly is checked; otherwise, <c>false</c>.
        /// </value>
        public bool IsUseImagesEvenlyChecked
        {
            get => this.isUseImagesEvenlyChecked;
            set
            {
                this.isUseImagesEvenlyChecked = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether square grid is selected.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this square is grid selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsSquareGridSelected
        {
            get => this.isSquareGridSelected;

            set
            {
                this.isSquareGridSelected = value;
                OnPropertyChanged();
                if (this.imageSource != null)
                {
                    this.createGridImageAsync();
                    this.createTriangleGridImageAsync();
                }

                this.GeneratePictureMosaicCommand.OnCanExecuteChanged();
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether zoom is selected.
        /// </summary>
        /// <value>
        ///     <c>true</c> if zoom is selected; otherwise, <c>false</c>.
        /// </value>
        public bool IsZoomSelected
        {
            get => this.isZoomSelected;
            set
            {
                this.isZoomSelected = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Gets or sets the selected image.
        /// </summary>
        /// <value>
        ///     The selected image.
        /// </value>
        public PaletteImageGenerator SelectedImage
        {
            get => this.selectedImage;
            set
            {
                this.selectedImage = value;
                OnPropertyChanged();
                this.isUsingSelectedImages = false;
                this.IsImageSelected = this.selectedImage != null;
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MainPageViewModel" /> class.
        /// </summary>
        public MainPageViewModel()
        {
            this.originalImageGenerator = new ImageGenerator();
            this.gridImageGenerator = new ImageGridGenerator();
            this.blockMosaicMaker = new BlockMosaicMaker();
            this.triangleMosaicMaker = new TriangleMosaicMaker();
            this.triangleGridImageGenerator = new TriangleGridGenerator();
            this.pictureMosaicMaker = new PictureMosaicMaker();
            this.palette = new ObservableCollection<PaletteImageGenerator>();
            this.GridSize = DefaultGridSize;
            this.CanSaveImage = false;
            this.isSquareGridSelected = true;
            this.IsBlackWhiteToggled = false;
            this.isUsingSelectedImages = false;

            this.loadCommands();
        }

        #endregion

        #region Methods

        private void loadCommands()
        {
            this.GenerateBlockMosaicCommand = CreateCommand(this.generateBlockMosaic, this.canGenerateBlockMosaic);
            this.GeneratePictureMosaicCommand =
                CreateCommand(this.generatePictureMosaic, this.canGeneratePictureMosaic);
            this.ClearPaletteImagesCommand = CreateCommand(this.clearPaletteImages, this.canClearPaletteImages);
        }

        private bool canClearPaletteImages(object obj)
        {
            return this.palette.Count > 0;
        }

        private void clearPaletteImages(object obj)
        {
            var images = this.Palette;
            images.Clear();
            this.IsUsingSelectedImages = false;
            this.Palette = images;
        }

        /// <summary>
        ///     Deletes the selected palette images.
        /// </summary>
        /// <param name="images">The selected images.</param>
        public void DeleteSelectedImages(ICollection<PaletteImageGenerator> images)
        {
            var selected = this.Palette;
            foreach (var current in images)
            {
                if (selected.Contains(current))
                {
                    selected.Remove(current);
                }
            }

            this.Palette = selected;
            this.IsUsingSelectedImages = false;
            this.SelectedImage = null;
        }

        private bool canGenerateBlockMosaic(object obj)
        {
            return this.originalImage != null;
        }

        private bool canGeneratePictureMosaic(object obj)
        {
            return this.isSquareGridSelected && this.palette.Count > 0 && this.originalImage != null;
        }

        private async void generateBlockMosaic(object obj)
        {
            if (this.IsSquareGridSelected)
            {
                await this.setAndDisplayMosaicImage(this.blockMosaicMaker);
            }
            else
            {
                await this.setAndDisplayMosaicImage(this.triangleMosaicMaker);
            }
        }

        private async void generatePictureMosaic(object obj)
        {
            await this.pictureMosaicMaker.SetSourceAsync(this.imageSource);
            await this.scalePaletteImagesAsync();
            this.pictureMosaicMaker.BlockLength = this.GridSize;
            this.pictureMosaicMaker.Palette = this.SelectedPalette;
            if (this.isUseImagesEvenlyChecked)
            {
                this.pictureMosaicMaker.GenerateMosaicUsingImagesEvenly();
            }
            else
            {
                this.pictureMosaicMaker.GenerateMosaic();
            }

            this.IsUsingSelectedImages = false;
            this.MosaicImage = await this.pictureMosaicMaker.GenerateImageAsync();
            this.pictureMosaicMaker.ConvertToBlackAndWhite();
            this.BlackAndWhiteMosaic = await this.pictureMosaicMaker.GenerateImageAsync();
            this.UpdateMosaicImage();
        }

        private async Task setAndDisplayMosaicImage(MosaicMaker mosaicMaker)
        {
            mosaicMaker.GenerateMosaic();

            this.MosaicImage = await mosaicMaker.GenerateImageAsync();
            
            mosaicMaker.ConvertToBlackAndWhite();
            this.BlackAndWhiteMosaic = await mosaicMaker.GenerateImageAsync();

            this.UpdateMosaicImage();
        }

        private async Task scalePaletteImagesAsync()
        {
            foreach (var image in this.Palette)
            {
                await image.ScaleImageSquare(this.GridSize);
            }
        }

        /// <summary>
        ///     Displays the original image asynchronous.
        /// </summary>
        /// <param name="source">The source of the image data.</param>
        /// <returns>The completed asynchronous operation.</returns>
        public async Task LoadImageSource(IRandomAccessStream source)
        {
            // TODO Can this method just be turned into a property?
            this.imageSource = source;

            await this.updateGeneratorSources(source);

            this.DisplayedMosaicImage = null;

            await this.createOriginalImageAsync();
            this.UpdateDisplayedImage();
        }

        private async Task updateGeneratorSources(IRandomAccessStream source)
        {
            await this.gridImageGenerator.SetSourceAsync(source);
            await this.triangleGridImageGenerator.SetSourceAsync(source);

            await this.blockMosaicMaker.SetSourceAsync(source);
            await this.triangleMosaicMaker.SetSourceAsync(source);
            await this.pictureMosaicMaker.SetSourceAsync(source);
        }

        /// <summary>
        ///     Adds the image to palette.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <returns>The completed asynchronous operation</returns>
        public async Task AddImageToPalette(IRandomAccessStream source)
        {
            var images = this.Palette;
            var paletteImage = new PaletteImageGenerator();
            await paletteImage.SetSourceAsync(source);
            images.Add(paletteImage);
            this.Palette = images;
            this.SelectedPalette = images;
            this.IsUsingSelectedImages = false;
            //TODO idk if can refactor because this is similar to GeneratePalette
        }

        /// <summary>
        ///     Updates the displayed image based on currently selected image parameters (grid on/off, grid type).
        /// </summary>
        public void UpdateDisplayedImage()
        {
            if (this.IsGridToggled)
            {
                if (this.isSquareGridSelected)
                {
                    this.DisplayedSourceImage = this.gridImage;
                }
                else
                {
                    this.DisplayedSourceImage = this.triangleGridImage;
                }
            }
            else
            {
                this.DisplayedSourceImage = this.originalImage;
            }
        }

        /// <summary>
        ///     Updates the Displayed mosaic image based on if black and white is selected
        /// </summary>
        public void UpdateMosaicImage()
        {
            if (this.IsBlackWhiteToggled)
            {
                this.DisplayedMosaicImage = this.blackAndWhiteMosaic;
            }
            else
            {
                this.DisplayedMosaicImage = this.mosaicImage;
            }
        }

        private async Task createOriginalImageAsync()
        {
            await this.originalImageGenerator.SetSourceAsync(this.imageSource);
            this.OriginalImage = await this.originalImageGenerator.GenerateImageAsync();
        }

        private async void createGridImageAsync()
        {
            await this.gridImageGenerator.SetSourceAsync(this.imageSource);
            this.gridImageGenerator.DrawGrid(this.GridSize);
            this.GridImage = await this.gridImageGenerator.GenerateImageAsync();
        }

        /// <summary>
        ///     Generates the palette.
        /// </summary>
        /// <param name="paletteSource">The palette source.</param>
        /// <returns>The completed asynchronous operation</returns>
        public async Task GeneratePalette(IReadOnlyList<IRandomAccessStream> paletteSource)
        {
            var paletteImages = new ObservableCollection<PaletteImageGenerator>();

            foreach (var source in paletteSource)
            {
                var paletteImage = new PaletteImageGenerator();
                await paletteImage.SetSourceAsync(source);
                paletteImages.Add(paletteImage);
            }

            this.IsUsingSelectedImages = false;
            this.Palette = paletteImages;
            this.SelectedPalette = paletteImages;
        }

        private async void createTriangleGridImageAsync()
        {
            await this.triangleGridImageGenerator.SetSourceAsync(this.imageSource);
            this.triangleGridImageGenerator.DrawGrid(this.GridSize);
            this.TriangleGridImage = await this.triangleGridImageGenerator.GenerateImageAsync();
        }

        /// <summary>
        ///     Writes the data asynchronous.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public async Task WriteDataAsync(StorageFile file)
        {
            var imageWriter = new ImageWriter();
            await imageWriter.WriteImageAsync(this.pictureMosaicMaker, file);
        }

        /// <summary>
        ///     Updates the selected palette.
        /// </summary>
        /// <param name="objects">The objects.</param>
        public void UpdateSelectedPalette(IList<object> objects)
        {
            if (this.IsUsingSelectedImages)
            {
                var selectedImages = new ObservableCollection<PaletteImageGenerator>();
                foreach (var current in objects)
                {
                    selectedImages.Add((PaletteImageGenerator) current);
                }

                this.SelectedPalette = selectedImages;
            }
        }

        #endregion
    }
}