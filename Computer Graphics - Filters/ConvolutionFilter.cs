using System.Collections.Generic;
using System.Threading;
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
        private static int NumberOfThreads = 6;

        public ConvolutionFilter(BitmapSource image, double[,] kernel, int anchorX, int anchorY, int offset, double divisor) : base(image)
        {
            Kernel = kernel;
            AnchorX = anchorX;
            AnchorY = anchorY;
            Offset = offset;
            Divisor = divisor;
        }
        //One thread multiplication
        private byte MultiplyByKernel(int pixelIdx)
        {
            double newPixelValue = 0;
            int stride = ToProcess.BackBufferStride;
            int valuesPerPixel = base.ToProcess.Format.BitsPerPixel / 8;
            for (int i = 0; i < Kernel.GetLength(0); i++)
            {
                for (int j = 0; j < Kernel.GetLength(1); j++)
                {
                    int offsetX = i - AnchorX;
                    int offsetY = j - AnchorY;
                    int idx = pixelIdx + (offsetY * stride + offsetX) * valuesPerPixel;
                    if (idx > 0 && idx < Pixels.Length)
                    {
                        newPixelValue += base.Pixels[idx] * Kernel[i, j];
                    }
                    else
                    {
                        newPixelValue += base.Pixels[pixelIdx] * Kernel[i, j];
                    }
                }
            }
            newPixelValue = Offset + newPixelValue / Divisor;
            return (byte)(newPixelValue > -1 && newPixelValue < 256 ? newPixelValue : (newPixelValue > -1 ? 255 : 0));
        }
        //Thread safe multiplication
        private byte ThreadSafeMultiplyByKernel(int pixelIdx, int stride, int valuesPerPixel, int kernelX, int kernelY, int pixelsLength)
        {
            double newPixelValue = 0;
            for (int i = 0; i < kernelX; i++)
            {
                for (int j = 0; j < kernelY; j++)
                {
                    int offsetX = i - AnchorX;
                    int offsetY = j - AnchorY;
                    int idx = pixelIdx + (offsetY * stride + offsetX) * valuesPerPixel;
                    if (idx > 0 && idx < pixelsLength)
                    {
                        newPixelValue += base.Pixels[idx] * Kernel[i, j];
                    }
                    else
                    {
                        newPixelValue += base.Pixels[pixelIdx] * Kernel[i, j];
                    }
                }
            }
            newPixelValue = Offset + newPixelValue / Divisor;
            return (byte)(newPixelValue > -1 && newPixelValue < 256 ? newPixelValue : (newPixelValue > -1 ? 255 : 0));
        }
        public BitmapSource FilterImage()
        {

            //SingleThreadImageProcess();
            MultithreadImageProcess(NumberOfThreads);
            base.Pixels = PixelsModified;
            base.WritePixels();
            return base.ToProcess;
        }
        
        private void SingleThreadImageProcess()
        {
            PixelsModified = new byte[base.Pixels.Length];
            for (int i = 0; i < base.Pixels.Length; i++)
            {
                PixelsModified[i] = MultiplyByKernel(i);
            }
        }
        private void MultithreadImageProcess(int NumberOfThreads)
        {
            PixelsModified = new byte[base.Pixels.Length];
            int offset = PixelsModified.Length / NumberOfThreads;
            int stride = ToProcess.BackBufferStride;
            int valuesPerPixel = base.ToProcess.Format.BitsPerPixel / 8;
            int kernelX = Kernel.GetLength(0);
            int kernelY = Kernel.GetLength(1);
            int pixelLength = base.Pixels.Length;
            List<Thread> threadPool = new List<Thread>(NumberOfThreads);
            for (int i = 0; i < NumberOfThreads; i++)
            {
                int startIdx = offset * i;
                int endIdx = offset * (i + 1);
                if (i != NumberOfThreads - 1)
                {
                    Thread t = new Thread(() => ProcessPart(startIdx,endIdx, stride, valuesPerPixel,kernelX, kernelY,pixelLength));
                    threadPool.Add(t);
                    t.Start();                   
                }
                else
                {
                    Thread t = new Thread(() => ProcessPart(startIdx, pixelLength, stride, valuesPerPixel, kernelX, kernelY, pixelLength));
                    threadPool.Add(t);
                    t.Start();                  
                }
            }
            foreach (Thread t in threadPool) {
                t.Join();
            }
        }
        public void ProcessPart(int startIdx, int endIdx, int stride, int valuesPerPixel, int kernelX, int kernelY, int pixelsLength)
        {
           for (int i = startIdx; i < endIdx; i++)
           {
                PixelsModified[i] = ThreadSafeMultiplyByKernel(i, stride, valuesPerPixel, kernelX, kernelY, pixelsLength);
           }
            
        }
    }
}
