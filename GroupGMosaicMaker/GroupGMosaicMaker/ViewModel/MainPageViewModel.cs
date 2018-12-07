﻿using System.Collections.Generic;
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

        private IRandomAccessStream imageSource;

        private ObservableCollection<PaletteImageGenerator> palette;
        private ObservableCollection<PaletteImageGenerator> selectedPalette;

        private WriteableBitmap originalImage;
        private readonly ImageGenerator originalImageGenerator;

        private WriteableBitmap gridImage;
        private readonly ImageGridGenerator gridImageOperator;

        private WriteableBitmap mosaicImage;
        private readonly BlockMosaicMaker blockMosaicMaker;
        private readonly PictureMosaicMaker pictureMosaicMaker;

        private WriteableBitmap displayedSourceImage;

        private WriteableBitmap triangleGridImage;
        private readonly TriangleGridGenerator triangleGridImageOperator;

        private int gridSize;
        private bool isSquareGridSelected;
        private bool isZoomSelected;

        private PaletteImageGenerator selectedImage;
        private bool isUsingSelectedImages;
        private bool isImageSelected;
        private int paletteCount;

        private WriteableBitmap blackAndWhiteMosaic;
        private WriteableBitmap displayedMosaicImage;

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
                if (value != null)
                {
                    this.CanSaveImage = true;
                }

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
        ///     Gets or sets the image source.
        /// </summary>
        /// <value>
        ///     The image source.
        /// </value>
        public WriteableBitmap ImageSource { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the grid is toggled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the grid is toggled; otherwise, <c>false</c>.
        /// </value>
        public bool IsGridToggled { get; set; }

        /// <summary>
        ///      Gets or sets a value indicating whether black and white is toggled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if black white is toggled; otherwise, <c>false</c>.
        /// </value>
        public bool IsBlackWhiteToggled { get; set; }

        private bool isUseImagesEvenlyChecked;

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
                if (this.selectedImage != null)
                {
                    this.IsImageSelected = true;
                }
                else
                {
                    this.IsImageSelected = false;
                }
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
            this.gridImageOperator = new ImageGridGenerator();
            this.blockMosaicMaker = new BlockMosaicMaker();
            this.triangleGridImageOperator = new TriangleGridGenerator();
            this.pictureMosaicMaker = new PictureMosaicMaker();
            this.palette = new ObservableCollection<PaletteImageGenerator>();
            this.gridSize = DefaultGridSize;
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
            await this.blockMosaicMaker.SetSourceAsync(this.imageSource);
            this.blockMosaicMaker.BlockLength = this.GridSize;
            this.blockMosaicMaker.GenerateMosaic();

            this.MosaicImage = await this.blockMosaicMaker.GenerateImageAsync();
            this.blockMosaicMaker.ConvertBlocksToBlackAndWhite();
            this.BlackAndWhiteMosaic = await this.blockMosaicMaker.GenerateImageAsync();
            this.UpdateMosaicImage();
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
            this.pictureMosaicMaker.ConvertBlocksToBlackAndWhite();
            this.BlackAndWhiteMosaic = await this.pictureMosaicMaker.GenerateImageAsync();
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
            this.imageSource = source;

            this.DisplayedMosaicImage = null;

            await this.createOriginalImageAsync();
            this.createGridImageAsync();
            this.createTriangleGridImageAsync();

            this.UpdateDisplayedImage();
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
            await this.gridImageOperator.SetSourceAsync(this.imageSource);
            this.gridImageOperator.DrawGrid(this.GridSize);
            this.GridImage = await this.gridImageOperator.GenerateImageAsync();
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
            await this.triangleGridImageOperator.SetSourceAsync(this.imageSource);
            this.triangleGridImageOperator.DrawGrid(this.GridSize);
            this.TriangleGridImage = await this.triangleGridImageOperator.GenerateImageAsync();
        }

        /// <summary>
        ///     Writes the data asynchronous.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public async Task WriteDataAsync(StorageFile file)
        {
            await ImageWriter.WriteImageAsync(this.pictureMosaicMaker, file);
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