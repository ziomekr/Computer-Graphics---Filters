
using System.Windows.Media.Imaging;

namespace Computer_Graphics___Filters
{
    class GaussianSmootheningFilter : ConvolutionFilter
    {
        static double[,] kernel = { { 0, 1, 0 }, { 1, 4, 1 }, { 0, 1, 0 } };
        static int anchorX = 1, anchorY = 1;
        static int offset = 0;
        static double divisor = 8;
        public GaussianSmootheningFilter(BitmapSource image) : base(image, kernel, anchorX, anchorY, offset, divisor){}
    }
}
