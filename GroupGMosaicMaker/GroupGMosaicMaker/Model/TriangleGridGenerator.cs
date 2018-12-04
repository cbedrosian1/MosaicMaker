using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;

namespace GroupGMosaicMaker.Model
{
    class TriangleGridGenerator : ImageGridGenerator
    {
        public override void DrawGrid(int length)
        {
            base.DrawGrid(length);
            var height = 0;
            for (var width = (int) this.Decoder.PixelWidth - length; width < this.Decoder.PixelWidth; width += length)
            {
                height++;
                this.SetPixelColor(width, height, Colors.White);
            }
        }

    }
}
