using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Computer_Graphics___Filters
{
    class OctreeColorQuantizationFilter : Filter
    {
        public Octree ColorTree { get; set; }
        public OctreeColorQuantizationFilter(BitmapSource image, uint MaxColors) : base(image) {
            ColorTree = new Octree(this.Pixels, MaxColors);
        }

        public BitmapSource FilterImage() {
            ColorTree.QuantizeImage();
            base.WritePixels();
            return base.ToProcess;
        }
    }
}
