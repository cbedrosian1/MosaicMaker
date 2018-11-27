using System;
using Windows.UI.Xaml.Data;

namespace GroupGMosaicMaker.Converter
{
    /// <summary>
    ///     Converts integers to strings and back
    /// </summary>
    /// <seealso cref="Windows.UI.Xaml.Data.IValueConverter" />
    public class StringToIntConverter : IValueConverter
    {
        #region Methods

        /// <summary>
        ///     Converts the specified value from an int to a string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>Returns a string from the int value</returns>
        /// <exception cref="NotImplementedException"></exception>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var number = (int) value;
            return number.ToString();
        }

        /// <summary>
        ///     Converts the value from a string to an int.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="targetType">Type of the target.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="language">The language.</param>
        /// <returns>Returns an int from the string value</returns>
        /// <exception cref="NotImplementedException"></exception>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var text = (string) value;
            var valueToReturn = 10;

            if (!string.IsNullOrEmpty(text))
            {
                valueToReturn = int.Parse(text);
            }

            return valueToReturn;
        }

        #endregion
    }
}