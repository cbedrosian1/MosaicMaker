using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.Streams;

namespace GroupGMosaicMaker.DataTier
{
    public class StreamFolderLoader
    {
        public async Task<IReadOnlyList<IRandomAccessStream>> LoadFolder(StorageFolder folder)
        {
            var files = await folder.GetFilesAsync();

            var streams = new List<IRandomAccessStream>();
            var fileLoader = new StreamFileLoader();

            foreach (var file in files)
            {
                streams.Add(await fileLoader.LoadFile(file));
            }

            return streams;
        }
    }
}