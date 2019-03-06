using System.Windows.Media.Imaging;

namespace Computer_Graphics___Filters
{
    class BlurFilter : ConvolutionFilter
    {
        static double[,] kernel = { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
        static int anchorX = 1, anchorY = 1;
        static int offset = 0;
        static double divisor = 9;
        public BlurFilter(BitmapSource image) : base(image, kernel, anchorX, anchorY, offset, divisor){}
    }
}
