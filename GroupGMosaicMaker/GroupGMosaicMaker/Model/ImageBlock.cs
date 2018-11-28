using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace GroupGMosaicMaker.Model
{
    public class ImageBlock
    {
        private readonly IList<Color> pixelColors;

        public ImageBlock(IList<Color> pixelColors)
        {
            this.pixelColors = pixelColors;
        }

        public Color CalculateAverageColor()
        {
            var averageR = (byte) this.pixelColors.Average(color => color.R);
            var averageG = (byte) this.pixelColors.Average(color => color.G);
            var averageB = (byte) this.pixelColors.Average(color => color.B);

            return Color.FromArgb(0, averageR, averageG, averageB);
        }
    }
}