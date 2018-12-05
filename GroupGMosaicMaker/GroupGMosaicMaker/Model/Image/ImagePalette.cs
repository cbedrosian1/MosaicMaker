using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace GroupGMosaicMaker.Model.Image
{
    class ImagePalette : ICollection<PaletteImageGenerator>
    {

        private IList<PaletteImageGenerator> images;

        public ImagePalette(IList<PaletteImageGenerator> images)
        {
            this.images = images;
        }

        public int Count => this.images.Count;

        public bool IsReadOnly => this.images.IsReadOnly;

        public void Add(PaletteImageGenerator item)
        {
            this.images.Add(item);
        }

        public void Clear()
        {
            this.images.Clear();
        }

        public bool Contains(PaletteImageGenerator item)
        {
            return this.images.Contains(item);
        }

        public void CopyTo(PaletteImageGenerator[] array, int arrayIndex)
        {
            this.images.CopyTo(array, arrayIndex);
        }

        public IEnumerator<PaletteImageGenerator> GetEnumerator()
        {
            return this.images.GetEnumerator();
        }

        public bool Remove(PaletteImageGenerator item)
        {
            return this.images.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.images.GetEnumerator();
        }
    }
}
