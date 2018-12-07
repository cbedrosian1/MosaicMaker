using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace GroupGMosaicMaker.DataTier
{
    public class StreamFileLoader
    {
        #region Methods

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