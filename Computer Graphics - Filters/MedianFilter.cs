using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Computer_Graphics___Filters
{
    class MedianFilter : ConvolutionFilter
    {
        static double[,] kernel = { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } };
        static int anchorX = 1, anchorY = 1;
        static int offset = 0;
        static double divisor = 1;
        public MedianFilter(BitmapSource image) : base(image, kernel, anchorX, anchorY, offset, divisor){ }

        //Thread safe median
        private byte ThreadSafeMedianKernel(int pixelIdx, int stride, int valuesPerPixel, int kernelX, int kernelY, int pixelsLength)
        {
            byte[] pixels = new byte[kernelX * kernelY];
            for (int i = 0; i < kernelX; i++)
            {
                for (int j = 0; j < kernelY; j++)
                {
                    int offsetX = i - AnchorX;
                    int offsetY = j - AnchorY;
                    int idx = pixelIdx + (offsetY * stride + offsetX) * valuesPerPixel;
                    if (idx > 0 && idx < pixelsLength)
                    {
                        pixels[i*kernelX+j] = base.Pixels[idx];
                    }
                    else
                    {
                        pixels[i * kernelX + j] = base.Pixels[pixelIdx];
                    }
                }
            }
            Array.Sort(pixels);
            if ((kernelX * kernelY) % 2 == 1)
            {
                int idx = (int)Math.Floor((double)(kernelX * kernelY / 2));
                return pixels[idx];
            }
            else {
                int idx = kernelX * kernelY / 2;
                return (byte)((pixels[idx] + pixels[idx - 1]) / 2);
            }
        }

        override public void ProcessPart(int startIdx, int endIdx, int stride, int valuesPerPixel, int kernelX, int kernelY, int pixelsLength)
        {
            for (int i = startIdx; i < endIdx; i++)
            {
                PixelsModified[i] = ThreadSafeMedianKernel(i, stride, valuesPerPixel, kernelX, kernelY, pixelsLength);
            }

        }
    }
}
