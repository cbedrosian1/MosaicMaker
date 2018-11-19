using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using GroupGMosaicMaker.Utilities;

namespace GroupGMosaicMaker.ViewModel
{
    /// <summary>
    ///     Generic ViewModel that implements INotifyPropertyChanged
    /// </summary>
    /// <seealso cref="System.ComponentModel.INotifyPropertyChanged" />
    public abstract class ViewModelGeneric : INotifyPropertyChanged
    {
        #region Methods

        /// <summary>
        ///     Occurs when a property value changes.
        /// </summary>
        /// <returns></returns>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        ///     Creates a command.
        /// </summary>
        /// <param name="execute">The execute.</param>
        /// <param name="canExecute">The can execute.</param>
        /// <returns>The created command.</returns>
        public virtual RelayCommand CreateCommand(Action<object> execute, Predicate<object> canExecute)
        {
            return new RelayCommand(execute, canExecute);
        }

        #endregion
    }
}