using System.Windows.Media.Imaging;

namespace Computer_Graphics___Filters
{

    class ConvolutionFilter : Filter
    {
        public double[,] Kernel { get; set; }
        public int AnchorX { get; set; }
        public int AnchorY { get; set; }
        public byte[] PixelsModified { get; set; }
        public int Offset { get; set; }
        public double Divisor { get; set; }

        
        public ConvolutionFilter(BitmapSource image, double[,] kernel, int anchorX, int anchorY, int offset, double divisor) : base(image)
        {
            Kernel = kernel;
            AnchorX = anchorX;
            AnchorY = anchorY;
            Offset = offset;
            Divisor = divisor;
        }

        private byte MultiplyByKernel(int pixelIdx) {
            double newPixelValue = 0;
            int stride = ToProcess.BackBufferStride;
            int valuesPerPixel = base.ToProcess.Format.BitsPerPixel / 8;
            for (int i = 0; i < Kernel.GetLength(0); i++) {
                for (int j = 0; j < Kernel.GetLength(1); j++) {
                    int offsetX = i - AnchorX;
                    int offsetY = j - AnchorY;
                    int idx = pixelIdx + (offsetY * stride + offsetX) * valuesPerPixel;
                    if (idx > 0 && idx < Pixels.Length){
                        newPixelValue += base.Pixels[idx] * Kernel[i, j];
                    }
                    else {
                        newPixelValue += base.Pixels[pixelIdx] * Kernel[i, j];     
                    }
                }
            }
            newPixelValue = Offset + newPixelValue / Divisor;
            return (byte)(newPixelValue > -1 && newPixelValue < 256 ? newPixelValue : (newPixelValue > -1 ? 255 : 0));
        }
        public BitmapSource FilterImage() {
            PixelsModified = new byte[base.Pixels.Length];
            for (int i = 0; i<base.Pixels.Length; i++){
                PixelsModified[i] = MultiplyByKernel(i);
            }
            base.Pixels = PixelsModified;
            base.WritePixels();
            return base.ToProcess;
        } 
    }
}
