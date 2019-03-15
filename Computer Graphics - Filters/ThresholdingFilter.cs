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
        double Threshold { get; set; }
        int K { get; set; }
        byte[] KValues { get; set; }
        public ThresholdingFilter(BitmapSource image, double threshold, int k) : base(image){
            Threshold = threshold;
            K = k;
            KValues = SetKValues();
        }

        public BitmapSource FilterImage() {

            for (int i = 0; i < Pixels.Length; i++)
            {
                if (Pixels[i] != 255)
                {
                    int closestKIndex = GetClosestKIndex(Pixels[i]);
                    if (Pixels[i] < KValues[closestKIndex] + (KValues[closestKIndex + 1] - KValues[closestKIndex]) * Threshold)
                    {
                        Pixels[i] = KValues[closestKIndex];
                    }
                    else
                    {
                        Pixels[i] = KValues[closestKIndex + 1];
                    }
                }
            }
            base.WritePixels();
            return base.ToProcess;
        }

        private byte[] SetKValues() {

            byte[] KValues = new byte[K];
            int step = 255 / (K - 1);
            for (int i = 0; i < K; i++) {
                KValues[i] = (byte)(step * i);
            }
            return KValues;
        }

        private int GetClosestKIndex(byte pixel) {
            for (int i = 0; i < K; i++) {
                if (KValues[i] > pixel)
                    return i - 1;
            }
            return -1;
        }
    }
}
