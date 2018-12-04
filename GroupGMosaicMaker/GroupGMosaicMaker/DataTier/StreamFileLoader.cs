using System;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using GroupGMosaicMaker.ViewModel;

namespace GroupGMosaicMaker.DataTier
{
    public class StreamFileLoader
    {
        public async Task<IRandomAccessStream> LoadFile(StorageFile file)
        {
            using (var fileStream = await file.OpenAsync(FileAccessMode.Read))
            {
                var clonedStream = fileStream.CloneStream();
                return clonedStream;
            }
        }
    }
}