using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Computer_Graphics___Filters
{
    class ThresholdingFilter : Filter
    {
        protected int K { get; set; }
        protected byte[] KValues { get; set; }
        protected double Threshold { get; set; }
        
        public ThresholdingFilter(BitmapSource image, double threshold, int k) : base(image){
            K = k;
            Threshold = threshold;
            KValues = SetKValues();
        }

        public virtual BitmapSource FilterImage() {
            
            for (int i = 0; i < Pixels.Length; i++)
            {
                if (Pixels[i] != 255)
                {
                    ProcessPixel(i);
                }
            }
            base.WritePixels();
            return base.ToProcess;
        }
        private byte[] SetKValues()
        {

            byte[] KValues = new byte[K];
            double step = (double)255 / (K - 1);
            for (int i = 0; i < K; i++)
            {
                KValues[i] = (byte)(step * i);
            }
            return KValues;
        }
        private int GetClosestKIndex(byte pixel) {
            for (int i = 0; i < K; i++) {
                if (KValues[i] > pixel)
                    return i - 1;
            }
            return K-2;
        }

        protected void ProcessPixel(int pixel_index) {
            int closestKIndex = GetClosestKIndex(Pixels[pixel_index]);
            if (Pixels[pixel_index] < KValues[closestKIndex] + (KValues[closestKIndex + 1] - KValues[closestKIndex]) * Threshold)
            {
                base.Pixels[pixel_index] = KValues[closestKIndex];
            }
            else
            {
                base.Pixels[pixel_index] = KValues[closestKIndex + 1];
            }
        }
    }
}
