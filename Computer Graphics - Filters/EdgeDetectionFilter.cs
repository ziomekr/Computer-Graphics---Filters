using System.Windows.Media.Imaging;

namespace Computer_Graphics___Filters
{
    class EdgeDetectionFilter : ConvolutionFilter
    {
        static double[,] kernel = { { 0, -1, 0 }, { 0, 1, 0 }, { 0, 0, 0 } };
        static int anchorX = 1, anchorY = 1;
        static int offset = 0;
        static double divisor = 1;
        public EdgeDetectionFilter(BitmapSource image) : base(image, kernel, anchorX, anchorY, offset, divisor) { }
    }
}
