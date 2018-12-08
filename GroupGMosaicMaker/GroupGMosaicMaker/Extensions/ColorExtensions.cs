using Windows.UI;

namespace GroupGMosaicMaker.Extensions
{
    /// <summary>
    ///     Extension methods for the Color struct.
    /// </summary>
    public static class ColorExtensions
    {
        #region Methods

        /// <summary>
        ///     Calculates the average RGB channel value.
        /// </summary>
        /// <param name="color">The color.</param>
        /// <returns>The average RGB channel value, from 0 to 255.</returns>
        public static double CalculateAverageRgbChannelValue(this Color color)
        {
            return (color.R + color.G + color.B) / 3.0;
        }

        #endregion
    }
}