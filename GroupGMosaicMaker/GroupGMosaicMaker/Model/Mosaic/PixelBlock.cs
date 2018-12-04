using System.Collections;
using System.Collections.Generic;
using Windows.UI;

namespace GroupGMosaicMaker.Model.Mosaic
{
    public class PixelBlock : IList<Color>
    {
        #region Data members

        private readonly IList<Color> pixelColors;

        #endregion

        #region Properties

        public int Count => this.pixelColors.Count;

        public bool IsReadOnly => this.pixelColors.IsReadOnly;

        public Color this[int index]
        {
            get => this.pixelColors[index];
            set => this.pixelColors[index] = value;
        }

        #endregion

        #region Constructors

        public PixelBlock()
        {
            this.pixelColors = new List<Color>();
        }

        public PixelBlock(IList<Color> pixelColors)
        {
            this.pixelColors = pixelColors;
        }

        #endregion

        #region Methods

        public int IndexOf(Color item)
        {
            return this.pixelColors.IndexOf(item);
        }

        public void Insert(int index, Color item)
        {
            this.pixelColors.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            this.pixelColors.RemoveAt(index);
        }

        public void Add(Color item)
        {
            this.pixelColors.Add(item);
        }

        public void Clear()
        {
            this.pixelColors.Clear();
        }

        public bool Contains(Color item)
        {
            return this.pixelColors.Contains(item);
        }

        public void CopyTo(Color[] array, int arrayIndex)
        {
            this.pixelColors.CopyTo(array, arrayIndex);
        }

        public bool Remove(Color item)
        {
            return this.pixelColors.Remove(item);
        }

        public IEnumerator<Color> GetEnumerator()
        {
            return this.pixelColors.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.pixelColors.GetEnumerator();
        }

        #endregion
    }
}