using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace GroupGMosaicMaker.DataTier
{
    /// <summary>
    ///     Loads a file
    /// </summary>
    public class StreamFileLoader
    {
        #region Methods

        /// <summary>
        ///     Loads the file.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns></returns>
        public async Task<IRandomAccessStream> LoadFile(StorageFile file)
        {
            using (var fileStream = await file.OpenAsync(FileAccessMode.Read))
            {
                var clonedStream = fileStream.CloneStream();
                return clonedStream;
            }
        }

        #endregion
    }
}