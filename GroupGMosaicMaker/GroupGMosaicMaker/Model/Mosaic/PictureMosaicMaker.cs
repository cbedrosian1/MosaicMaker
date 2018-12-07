﻿using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using GroupGMosaicMaker.Model.Image;

namespace GroupGMosaicMaker.Model.Mosaic
{
    /// <summary>
    ///     Responsible for generating picture mosaics of the source image with a given palette of images.
    /// </summary>
    /// <seealso cref="GroupGMosaicMaker.Model.Mosaic.BlockMosaicMaker" />
    public class PictureMosaicMaker : BlockMosaicMaker
    {
        #region Data members

        private ICollection<PaletteImageGenerator> palette;
        private IDictionary<PaletteImageGenerator, Color> averageColorsByPaletteImage;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the image palette that the mosaic will be constructed out of.
        /// </summary>
        /// <value>
        ///     The palette.
        /// </value>
        public ICollection<PaletteImageGenerator> Palette
        {
            get => this.palette;
            set
            {
                this.palette = value;
                this.averageColorsByPaletteImage = this.CalculateAverageColorsFor(this.palette);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PictureMosaicMaker" /> class.
        /// </summary>
        public PictureMosaicMaker()
        {
            this.palette = new List<PaletteImageGenerator>();
            this.averageColorsByPaletteImage = new Dictionary<PaletteImageGenerator, Color>();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Generates the mosaic using each palette image once before using others again
        /// </summary>
        public void GenerateMosaicUsingImagesEvenly()
        {
            var imagesWithAvgColor = this.copyPalette(this.averageColorsByPaletteImage);

            for (var x = 0; x < Decoder.PixelHeight; x += BlockLength)
            {
                for (var y = 0; y < Decoder.PixelWidth; y += BlockLength)
                {
                    if (this.averageColorsByPaletteImage.Count == 0)
                    {
                        this.averageColorsByPaletteImage = this.copyPalette(imagesWithAvgColor);
                    }

                    this.generateBlockWhenUsingImagesEvenly(x, y);
                }
            }
        }

        private IDictionary<PaletteImageGenerator, Color> copyPalette(
            IDictionary<PaletteImageGenerator, Color> imagesWithAvgColor)
        {
            var images = new Dictionary<PaletteImageGenerator, Color>();
            foreach (var current in imagesWithAvgColor)
            {
                images.Add(current.Key, current.Value);
            }

            return images;
        }

        private void generateBlockWhenUsingImagesEvenly(int x, int y)
        {
            var currentBlock = FindSingleBlock(x, y);
            var currentBlockColor = currentBlock.CalculateAverageColor();
            var closestImage = this.findClosestPaletteImage(currentBlockColor);

            this.mapImageToBlock(x, y, closestImage);
            this.averageColorsByPaletteImage.Remove(closestImage);
        }

        /// <summary>
        ///     Generates the (x, y)'th block of the mosaic. Override this method to change the functionality of how each block is
        ///     generated.
        /// </summary>
        /// <param name="x">The row.</param>
        /// <param name="y">The column.</param>
        protected override void GenerateMosaicBlock(int x, int y)
        {
            var currentBlock = FindSingleBlock(x, y);
            var currentBlockColor = currentBlock.CalculateAverageColor();
            var closestImage = this.findClosestPaletteImage(currentBlockColor);

            this.mapImageToBlock(x, y, closestImage);
        }

        private IDictionary<PaletteImageGenerator, Color> CalculateAverageColorsFor(
            ICollection<PaletteImageGenerator> paletteToMap)
        {
            var paletteAverageColors = new Dictionary<PaletteImageGenerator, Color>();
            foreach (var paletteImage in paletteToMap)
            {
                var averageColor = paletteImage.PixelBlock.CalculateAverageColor();
                paletteAverageColors.Add(paletteImage, averageColor);
            }

            return paletteAverageColors;
        }

        private PaletteImageGenerator findClosestPaletteImage(Color sourceColor)
        {
            var colorComparisonsByImage = new Dictionary<PaletteImageGenerator, int>();
            foreach (var paletteColorPair in this.averageColorsByPaletteImage)
            {
                var currentColor = paletteColorPair.Value;
                var comparison = this.calculateColorComparison(sourceColor, currentColor);

                colorComparisonsByImage.Add(paletteColorPair.Key, comparison);
            }

            var imagesOrderedByComparison = colorComparisonsByImage.OrderBy(kvp => kvp.Value);

            var mostSimilarPair = imagesOrderedByComparison.First();
            return mostSimilarPair.Key;
        }

        private int calculateColorComparison(Color sourceColor, Color comparedColor)
        {
            var rComparison = Math.Abs(sourceColor.R - comparedColor.R);
            var gComparison = Math.Abs(sourceColor.G - comparedColor.G);
            var bComparison = Math.Abs(sourceColor.B - comparedColor.B);

            return rComparison + gComparison + bComparison;
        }

        private void mapImageToBlock(int startX, int startY, ImageGenerator paletteImage)
        {
            for (var y = startY; y < startY + BlockLength && y < Decoder.PixelWidth; y++)
            {
                for (var x = startX; x < startX + BlockLength && x < Decoder.PixelHeight; x++)
                {
                    var currentPaletteColor = paletteImage.FindPixelColor(x - startX, y - startY);
                    SetPixelColor(x, y, currentPaletteColor);
                }
            }
        }

        /// <summary>
        ///     Converts the blocks to black and white.
        /// </summary>
        public override void ConvertBlocksToBlackAndWhite()
        {
            for (var x = 0; x < Decoder.PixelHeight; x += BlockLength)
            {
                for (var y = 0; y < Decoder.PixelWidth; y += BlockLength)
                {
                    this.convertPixelToBlackAndWhite(x, y);
                }
            }
        }

        private void convertPixelToBlackAndWhite(int startX, int startY)
        {
            for (var y = startY; y < startY + BlockLength && y < Decoder.PixelWidth; y++)
            {
                for (var x = startX; x < startX + BlockLength && x < Decoder.PixelHeight; x++)
                {
                    var color = FindPixelColor(x, y);
                    var average = (color.R + color.B + color.G) / NumberOfColorValues;
                    if (average > HalfBetweenBlackAndWhite)
                    {
                        SetPixelColor(x, y, Colors.White);
                    }
                    else
                    {
                        SetPixelColor(x, y, Colors.Black);
                    }
                }
            }
        }

        #endregion
    }
}