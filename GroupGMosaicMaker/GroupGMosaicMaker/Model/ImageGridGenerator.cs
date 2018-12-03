using System.Threading.Tasks;
using Windows.Devices.AllJoyn;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace GroupGMosaicMaker.Model
{
    /// <summary>
    ///     Responsible for drawing square grids of varying sizes onto the given bitmap image.
    /// </summary>
    /// <seealso cref="ImageGenerator" />
    public class ImageGridGenerator : ImageGenerator
    {
        public void DrawGrid(int length)
        {
            for (var i = 0; i < this.Decoder.PixelHeight; i += length)
            {
                for (var j = 0; j < this.Decoder.PixelWidth; j++)
                {
                    this.SetPixelColor(i, j, Colors.White);
                }
            }

            for (var i = 0; i < this.Decoder.PixelHeight; i++)
            {
                for (var j = 0; j < this.Decoder.PixelWidth; j += length)
                {
                    this.SetPixelColor(i, j, Colors.White);
                }
            }
        }
    }
}