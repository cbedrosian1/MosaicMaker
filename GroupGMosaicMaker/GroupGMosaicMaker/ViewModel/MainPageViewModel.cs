﻿using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using GroupGMosaicMaker.DataTier;
using GroupGMosaicMaker.Model;
using GroupGMosaicMaker.Utilities;
using System;
using System.Collections.Generic;

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

        private IList<PaletteImageGenerator> palette;

        private WriteableBitmap originalImage;
        private readonly ImageGenerator originalImageGenerator;

        private WriteableBitmap gridImage;
        private readonly ImageGridGenerator gridImageOperator;

        private WriteableBitmap mosaicImage;
        private readonly BlockMosaicMaker blockMosaicMaker;
        private readonly PictureMosaicMaker pictureMosaicMaker;
        

        private WriteableBitmap displayedImage;

        private int gridSize;
        private bool isGridToggled;

        #endregion

        #region Properties

        public IList<PaletteImageGenerator> Palette
        {
            get => this.palette;
            set
            {
                this.palette = value;
                this.GeneratePictureMosaicCommand.OnCanExecuteChanged();
                OnPropertyChanged();
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
                if (this.imageSource != null)
                {
                    this.createGridImageAsync(this.imageSource);
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
        public bool IsGridToggled
        {
            get => this.isGridToggled;
            set => this.isGridToggled = value;
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
            this.pictureMosaicMaker = new PictureMosaicMaker();

            this.gridSize = 10;
            this.canSaveImage = false;

            this.loadCommands();
        }

        #region Methods

        private void loadCommands()
        {
            this.GenerateBlockMosaicCommand = CreateCommand(this.generateBlockMosaic, this.canGenerateBlockMosaic);
            this.GeneratePictureMosaicCommand = CreateCommand(this.generatePictureMosaic, this.canGeneratePictureMosaic);
        }

        private bool canGenerateBlockMosaic(object obj)
        {
            return this.OriginalImage != null;
        }

        private bool canGeneratePictureMosaic(object obj)
        {
            return this.palette != null;
        }

        private async void generateBlockMosaic(object obj)
        {
            //TODO might have to create separate command for picture mosaic
            await this.blockMosaicMaker.SetSourceAsync(this.imageSource);
            this.blockMosaicMaker.BlockLength = this.GridSize;
            this.blockMosaicMaker.GenerateMosaic();

            this.MosaicImage = await this.blockMosaicMaker.GenerateImageAsync();
        }

        private async void generatePictureMosaic(object obj)
        {
            await this.pictureMosaicMaker.SetSourceAsync(this.imageSource);

            this.pictureMosaicMaker.BlockLength = this.GridSize;
            this.pictureMosaicMaker.Palette = this.palette;
            this.pictureMosaicMaker.GenerateMosaic();

            this.MosaicImage = await this.pictureMosaicMaker.GenerateImageAsync();
        }

        /// <summary>
        ///     Displays the original image asynchronous.
        /// </summary>
        /// <param name="imageSource">The source of the image data.</param>
        /// <returns>The completed asynchronous operation.</returns>
        public async Task CreateImages(IRandomAccessStream imageSource)
        {
            this.imageSource = imageSource;
            
            await this.createOriginalImageAsync(imageSource);
            this.createGridImageAsync(imageSource);

            this.displayImageOnCreation();
            
        }

        private void displayImageOnCreation()
        {
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
            await this.originalImageGenerator.SetSourceAsync(imageSource);
            this.OriginalImage = await this.originalImageGenerator.GenerateImageAsync();
            this.CanSaveImage = true;
        }

        private async void createGridImageAsync(IRandomAccessStream imageSource)
        {
            await this.gridImageOperator.SetSourceAsync(imageSource);
            this.gridImageOperator.DrawGrid(this.GridSize);
            this.GridImage = await this.gridImageOperator.GenerateImageAsync();
        }

        public async Task GeneratePalette(IReadOnlyList<IRandomAccessStream> paletteSource)
        {
            this.palette = new List<PaletteImageGenerator>();
            foreach (var source in paletteSource)
            {
                var paletteImage = new PaletteImageGenerator();
                await paletteImage.SetSourceAsync(source);
                await paletteImage.ScaleImage(this.gridSize, this.gridSize);
                this.palette.Add(paletteImage);
            }
        }

        /// <summary>
        /// Writes the data asynchronous.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public async Task WriteDataAsync(StorageFile file)
        {
            
            await ImageWriter.WriteImageAsync(this.pictureMosaicMaker, file);
        }


        #endregion
    }
}