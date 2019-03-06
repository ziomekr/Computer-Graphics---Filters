using System;
using System.Windows.Media.Imaging;

namespace Computer_Graphics___Filters
{
    class Filter
    {
        protected WriteableBitmap ToProcess { get; set; }
        protected byte[] Pixels { get; set; }
        public Filter(BitmapSource image)
        {
            if (image != null)
            {
                ToProcess = new WriteableBitmap(image);
                Pixels = new byte[ToProcess.PixelHeight * ToProcess.PixelWidth * ToProcess.Format.BitsPerPixel / 8];
                ToProcess.CopyPixels(Pixels, ToProcess.BackBufferStride, 0);
            }
        }

        public void ChangeImage(BitmapSource image) {
            ToProcess = new WriteableBitmap(image);
            Pixels = new byte[ToProcess.PixelHeight * ToProcess.PixelWidth * ToProcess.Format.BitsPerPixel / 8];
            ToProcess.CopyPixels(Pixels, ToProcess.BackBufferStride, 0);
        }

        protected void WritePixels()
        {
            ToProcess.WritePixels(new System.Windows.Int32Rect(0, 0, ToProcess.PixelWidth, ToProcess.PixelHeight), Pixels, ToProcess.BackBufferStride, 0);
        }
    }
}
