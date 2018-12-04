using Windows.UI;

namespace GroupGMosaicMaker.Model
{
    public class PaletteImageGenerator : ImageGenerator
    {
        public PixelBlock Pixels { get; set; }
        public Color AverageColor { get; set; }
    }
}