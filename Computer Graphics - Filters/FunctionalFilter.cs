using System;
using System.Windows.Media.Imaging;

namespace Computer_Graphics___Filters
{
    class FunctionalFilter : Filter
    {
  
        public FunctionalFilter(BitmapSource image) : base(image){}

       
        //Inversion filter
        public BitmapSource Inversion() {
            
            for (int i = 0; i < Pixels.Length; i++) {
                Pixels[i] = (byte)(255 - Pixels[i]);
            }
            base.WritePixels();
            return ToProcess;
        }

        public BitmapSource BrightnessCorrection(int bias) {
            for (int i = 0; i < Pixels.Length; i++)
            {
                int newPixelValue = Pixels[i] + bias;
                Pixels[i] = (byte)((newPixelValue > -1 && newPixelValue < 256) ? newPixelValue : (newPixelValue > -1 ? 255 : 0));
            }
            base.WritePixels();
            return ToProcess;
        }

        public BitmapSource ContrastEnhancement(double gain)
        {
            for (int i = 0; i < Pixels.Length; i++)
            {
                //double newPixelValue = (((double)pixels[i]/255) * gain)*255;
                double newPixelValue = 128 - (1+gain) * (128 - Pixels[i]);
                Pixels[i] = (byte)((newPixelValue > -1 && newPixelValue < 256) ? newPixelValue : (newPixelValue > -1 ? 255 : 0));
            }
            base.WritePixels();
            return ToProcess;
        }

        public BitmapSource GammaCorrection(double gamma)
        {
            for (int i = 0; i < Pixels.Length; i++)
            {
                Pixels[i] = (byte)(Math.Pow((double)Pixels[i] / 255, 1/gamma) * 255);
            }
            base.WritePixels();
            return ToProcess;
        }
    }
}
