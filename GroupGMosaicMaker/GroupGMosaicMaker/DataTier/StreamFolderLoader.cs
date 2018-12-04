using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.Streams;

namespace GroupGMosaicMaker.DataTier
{
    public class StreamFolderLoader
    {
        private static IReadOnlyCollection<string> ValidFileExtensions = new ReadOnlyCollection<string>(
            new List<string> {".bmp", ".jpg", "png" }
        );

        public async Task<IReadOnlyList<IRandomAccessStream>> LoadFolder(StorageFolder folder)
        {
            var files = await folder.GetFilesAsync();

            var streams = new List<IRandomAccessStream>();
            var fileLoader = new StreamFileLoader();

            foreach (var file in files)
            {
                if (this.fileIsImage(file))
                {
                    streams.Add(await fileLoader.LoadFile(file));
                }
            }

            return streams;
        }

        private bool fileIsImage(StorageFile file)
        {
            var fileType = file.FileType;
            foreach (var extension in ValidFileExtensions)
            {
                if (fileType == extension)
                {
                    return true;
                }
            }

            return false;
        }
    }
}