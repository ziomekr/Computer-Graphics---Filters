using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Computer_Graphics___Filters
{
    class RGB
    {
        public byte Red { get; set; }
        public byte Green { get; set; }
        public byte Blue { get; set; }

        public RGB() { this.Red = this.Green = this.Blue = 0; }
        public RGB(byte Red, byte Green, byte Blue) { this.Red = Red; this.Green = Green; this.Blue = Blue; }
    }
}
