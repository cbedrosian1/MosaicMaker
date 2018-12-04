using System.Text;
using Windows.UI;

namespace GroupGMosaicMaker.Model.Grid
{
    class TriangleGridGenerator : ImageGridGenerator
    {
        public override void DrawGrid(int length)
        {
            base.DrawGrid(length);  

            for (var i = 0; i < Decoder.PixelHeight; i += length)
            {
                for (var j = 0; j < Decoder.PixelWidth; j += length)
                {
                    this.drawDiagonal(i, j, length);
                }
            }
        }

        private void drawDiagonal(int startX, int startY, int blockLength)
        {
            var x = startX;
            for (var y = startY; y < startY + blockLength && y < Decoder.PixelWidth; y++)
            {
                SetPixelColor(x, y, Colors.White);
                x++;
            }
        }
    }
}
