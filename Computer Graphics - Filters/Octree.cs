using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace Computer_Graphics___Filters
{
    class Octree
    {

        static byte[] Bitmasks = { 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80 };
        const byte ColorBits = 8;
        const int BytePerPixel = 4;
        static byte TreeDepth = 8;
        static byte LeafLevel = TreeDepth;
        private uint LeavesCount { get; set; }
        public uint MaxColors { get; set; }
        OctreeNode Root = new OctreeNode(1);
        List<Stack<OctreeNode>> ReducibleNodes = new List<Stack<OctreeNode>>(Enumerable.Range(1, TreeDepth).Select(x => new Stack<OctreeNode>()));
        RGB[] ColorTable;
        byte[] Pixels;
        public int CurrentColorTableIndex { get; set; }
        class OctreeNode
        {

            public int Level { get; set; }
            public bool IsLeaf { get; set; }
            public int Index { get; set; }
            public ulong PixelsCount { get; set; }
            public ulong RSum { get; set; }
            public ulong GSum { get; set; }
            public ulong BSum { get; set; }
            public RGB Color { get; set; }
            public List<OctreeNode> Children { get; set; }
            public OctreeNode(int Level)
            {
                this.Level = Level;
                this.RSum = this.GSum = this.BSum = 0;
                this.PixelsCount = 0;
                if (this.Level == LeafLevel)
                {
                    this.IsLeaf = true;
                }
                this.Children = new List<OctreeNode>(ColorBits);
                for (int i = 0; i < ColorBits; i++)
                {
                    this.Children.Add(null);
                }
            }
        }
        public Octree(byte[] Pixels, uint Colors) : base()
        {
            this.LeavesCount = 0;
            this.CurrentColorTableIndex = 0;
            ReducibleNodes[0].Push(Root);
            MaxColors = Colors;
            ColorTable = new RGB[MaxColors];
            this.Pixels = Pixels;
            BuildTree();
            MakeColorTable(Root);
        }
        private void BuildTree() {
            
            for (int i = 0; i < Pixels.Length / BytePerPixel; i++)
            {
                RGB Color = new RGB(Pixels[i * BytePerPixel + 2], Pixels[i * BytePerPixel + 1], Pixels[i * BytePerPixel]);
                InsertColor(Root, Color, 1);
                while (LeavesCount > MaxColors)
                {
                    ReduceTree();
                }
            }
        }
        
        private void InsertColor(OctreeNode Node, RGB Color, int Depth) {
          
            if (Node.IsLeaf)
            {
                Node.PixelsCount += 1;
                Node.RSum += Color.Red;
                Node.GSum += Color.Green;
                Node.BSum += Color.Blue;
            }
            else {
                int Branch = GetBranchLevel(Color, TreeDepth - Depth);
                if (Node.Children[Branch] == null) {
                    Node.Children[Branch] = new OctreeNode(Depth+1);
                    if (Node.Children[Branch].IsLeaf)
                    {
                        this.LeavesCount += 1;
                    }
                    else
                    {
                        MakeNodeReducible(Node.Children[Branch]);
                    }
                }
                InsertColor(Node.Children[Branch], Color, Depth+1);
            }

        }

        private void ReduceTree() {
            OctreeNode Node = GetReducibleNode();
            uint ChildrenCount = 0;
            for (int i = 0; i < ColorBits; i++) {
                if (Node.Children[i] != null) {
                    Node.RSum += Node.Children[i].RSum;
                    Node.GSum += Node.Children[i].GSum;
                    Node.BSum += Node.Children[i].BSum;
                    Node.PixelsCount += Node.Children[i].PixelsCount;
                    ChildrenCount += 1;
                }
            }
            Node.IsLeaf = true;
            LeavesCount -= (ChildrenCount - 1);
        }

        private int GetBit(byte Source, int BitIdx) {
            return (Source & Bitmasks[BitIdx]) >> BitIdx;
        }

        private int GetBranchLevel(RGB Color, int Depth) {
            return (GetBit(Color.Red, Depth) << 2 | GetBit(Color.Green, Depth) << 1 | GetBit(Color.Blue, Depth));
        }

        private void MakeNodeReducible(OctreeNode Node) {
            this.ReducibleNodes[Node.Level].Push(Node);
        }

        private OctreeNode GetReducibleNode() {
            while (!ReducibleNodes[LeafLevel - 1].Any())
                LeafLevel -= 1;
            return ReducibleNodes[LeafLevel - 1].Pop();
        }

        private void MakeColorTable(OctreeNode Node) {
            if (Node.IsLeaf)
            {
                RGB Color = new RGB();
                Color.Red = (byte)(Node.RSum / Node.PixelsCount);
                Color.Green = (byte)(Node.GSum / Node.PixelsCount);
                Color.Blue = (byte)(Node.BSum / Node.PixelsCount);
                ColorTable[CurrentColorTableIndex] = Color;
                Node.Index = CurrentColorTableIndex;
                CurrentColorTableIndex += 1;
            }
            else {
                for (int i = 0; i < ColorBits; i++) {
                    if (Node.Children[i] != null) {
                        MakeColorTable(Node.Children[i]);
                    }
                }
            }
        }

        private int QuantizeColor(OctreeNode Node, RGB Color) {
            if (Node.IsLeaf)
            {
                return Node.Index;
            }
            else {
                return QuantizeColor(Node.Children[GetBranchLevel(Color, TreeDepth - Node.Level)], Color);
            }
        }

        public void QuantizeImage() {
            for (int i = 0; i < Pixels.Length / BytePerPixel; i++)
            {

                RGB Color = new RGB(Pixels[i * BytePerPixel + 2], Pixels[i * BytePerPixel + 1], Pixels[i * BytePerPixel]);
                int idx = QuantizeColor(Root, Color);
                Pixels[i * BytePerPixel + 2] = ColorTable[idx].Red;
                Pixels[i * BytePerPixel + 1] = ColorTable[idx].Green;
                Pixels[i * BytePerPixel] = ColorTable[idx].Blue;
            }
        }
    }
}
