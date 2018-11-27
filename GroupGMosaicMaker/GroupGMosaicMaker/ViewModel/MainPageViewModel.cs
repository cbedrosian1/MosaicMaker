﻿using System.Threading.Tasks;
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

        public RelayCommand GenerateMosaicCommand { get; set; }

        public RelayCommand GenerateGridImage { get; set; }

        #endregion

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

        private void generateGrid(object obj)
        {
            throw new NotImplementedException();
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
            this.OriginalImage = await this.gridImageOperator.GenerateModifiedImageAsync();
        }

        public async Task WriteDataAsync(StorageFile file)
        {
            await ImageWriter.WriteImageAsync(this.gridImageOperator, file);
        }

        #endregion
    }
}