using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Computer_Graphics___Filters
{
    class RandomDitheringFilter : ThresholdingFilter
    {

        private Random RandomThreshold = new Random();
        private int BytePerPixel { get; set; }
        public RandomDitheringFilter(BitmapSource image, int k) : base(image, 0, k) {
            this.BytePerPixel = base.ToProcess.Format.BitsPerPixel / sizeof(byte);
        }
        

        public override BitmapSource FilterImage() {

            for (int i = 0; i < Pixels.Length/this.BytePerPixel; i++)
            {
                if (Pixels[i* this.BytePerPixel] != 255)
                {
                    Threshold = RandomThreshold.NextDouble();
                    for (int j = 0; j < this.BytePerPixel; j++) {
                        base.ProcessPixel(i * this.BytePerPixel + j);
                    }

                }
            }
            base.WritePixels();
            return base.ToProcess;
        }
    }
}
