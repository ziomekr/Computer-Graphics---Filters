using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Computer_Graphics___Filters
{
    class PaletteBMPCreator
    {
        static readonly int ColorSquareDimension = 40;
        static readonly int BitmapWidth = 220;
        static readonly int PaletteColumns = BitmapWidth / ColorSquareDimension;
        public WriteableBitmap ColorPaletteBMP { get; set; }
        public PaletteBMPCreator(RGB[] Palette, int ColorsCount)
        {
            WriteableBitmap PaletteBitmap = new WriteableBitmap(BitmapWidth, ColorSquareDimension*((int)Math.Ceiling((double)ColorsCount/PaletteColumns)), 96, 96, System.Windows.Media.PixelFormats.Bgra32, null);
            byte[] Pixels = new byte[PaletteBitmap.PixelHeight * PaletteBitmap.PixelWidth * 4];
            int PaletteIndex = 0;
            for (int PaletteRow = 0; PaletteRow < ColorsCount; PaletteRow++)
            {
                for (int PaletteColumn = 0; PaletteColumn < PaletteColumns; PaletteColumn++)
                {
                    if (PaletteIndex < ColorsCount)
                    {
                        for (int ColorSquarePixelWidth = 0; ColorSquarePixelWidth < ColorSquareDimension; ColorSquarePixelWidth++)
                        {

                            for (int ColorSquarePixelHeight = 0; ColorSquarePixelHeight < ColorSquareDimension; ColorSquarePixelHeight++)
                            {
                                Pixels[PaletteRow * PaletteBitmap.BackBufferStride * ColorSquareDimension + PaletteColumn * ColorSquareDimension * 4 + ColorSquarePixelWidth * 4 + ColorSquarePixelHeight * PaletteBitmap.BackBufferStride] = Palette[PaletteIndex].Blue;
                                Pixels[PaletteRow * PaletteBitmap.BackBufferStride * ColorSquareDimension + PaletteColumn * ColorSquareDimension * 4 + ColorSquarePixelWidth * 4 + ColorSquarePixelHeight * PaletteBitmap.BackBufferStride + 1] = Palette[PaletteIndex].Green;
                                Pixels[PaletteRow * PaletteBitmap.BackBufferStride * ColorSquareDimension + PaletteColumn * ColorSquareDimension * 4 + ColorSquarePixelWidth * 4 + ColorSquarePixelHeight * PaletteBitmap.BackBufferStride + 2] = Palette[PaletteIndex].Red;
                                Pixels[PaletteRow * PaletteBitmap.BackBufferStride * ColorSquareDimension + PaletteColumn * ColorSquareDimension * 4 + ColorSquarePixelWidth * 4 + ColorSquarePixelHeight * PaletteBitmap.BackBufferStride + 3] = 255;
                            }
                        }
                    }
                    else {
                        PaletteBitmap.WritePixels(new Int32Rect(0, 0, PaletteBitmap.PixelWidth, PaletteBitmap.PixelHeight), Pixels, PaletteBitmap.BackBufferStride, 0);
                        ColorPaletteBMP = PaletteBitmap;
                        return;
                    }
                    PaletteIndex += 1;
                }
            }
        }
    }
}
