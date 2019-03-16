using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Computer_Graphics___Filters
{
    class RGBToGrayscaleFilter : Filter
    {
        private const double RChannelWeight = 0.2126;
        private const double GChannelWeight = 0.7152;
        private const double BChannelWeight = 0.0722;

        public RGBToGrayscaleFilter(BitmapSource image) : base(image)  {}
        public BitmapSource FilterImage() {
            for (int i = 0; i < Pixels.Length / 4; i++)
            {

                byte newPixelValue = GetNewPixelValue(i * 4);
                Pixels[i * 4] = newPixelValue;
                Pixels[i*4+1] = newPixelValue;
                Pixels[i*4+2] = newPixelValue;

            }
            base.WritePixels();
            return this.ToProcess;
        }

        private byte GetNewPixelValue(int pixel_index) {
            double linearPixelValue = BChannelWeight * (double)Pixels[pixel_index] / 255 + GChannelWeight * (double)Pixels[pixel_index + 1] / 255 + RChannelWeight * (double)Pixels[pixel_index + 2] / 255;
            if (linearPixelValue > 0.0031308)
            {
                return (byte)((1.055 * Math.Pow(linearPixelValue, 1 / 2.4) - 0.055)*255);
            }
            else {
                return (byte)(12.92 * linearPixelValue * 255);
            }
        }

     
    }
}
