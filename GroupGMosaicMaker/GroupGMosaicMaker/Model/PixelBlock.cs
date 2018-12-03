using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace GroupGMosaicMaker.Model
{
    public class PixelBlock : IList<Color>
    {
        private readonly IList<Color> pixelColors;

        public int Count => pixelColors.Count;

        public bool IsReadOnly => pixelColors.IsReadOnly;

        public Color this[int index] { get => pixelColors[index]; set => pixelColors[index] = value; }

        public PixelBlock(IList<Color> pixelColors)
        {
            this.pixelColors = pixelColors;
        }

        public int IndexOf(Color item)
        {
            return pixelColors.IndexOf(item);
        }

        public void Insert(int index, Color item)
        {
            pixelColors.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            pixelColors.RemoveAt(index);
        }

        public void Add(Color item)
        {
            pixelColors.Add(item);
        }

        public void Clear()
        {
            pixelColors.Clear();
        }

        public bool Contains(Color item)
        {
            return pixelColors.Contains(item);
        }

        public void CopyTo(Color[] array, int arrayIndex)
        {
            pixelColors.CopyTo(array, arrayIndex);
        }

        public bool Remove(Color item)
        {
            return pixelColors.Remove(item);
        }

        public IEnumerator<Color> GetEnumerator()
        {
            return pixelColors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return pixelColors.GetEnumerator();
        }
    }
}