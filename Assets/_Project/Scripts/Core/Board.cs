using System;

namespace GemBlast.Core
{
    public class Board
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        
        private BlockData[] _blocks;

        public Board(int width, int height)
        {
            Width = width;
            Height = height;
            _blocks = new BlockData[width * height];
        }

        public void SetBlock(int x, int y, BlockData block)
        {
            if (IsValidCoordinate(x, y))
            {
                _blocks[GetIndex(x, y)] = block;
            }
        }

        public BlockData GetBlock(int x, int y)
        {
            if (!IsValidCoordinate(x, y)) return default;
            return _blocks[GetIndex(x, y)];
        }

        public bool IsValidCoordinate(int x, int y)
        {
            return x >= 0 && x < Width && y >= 0 && y < Height;
        }

        private int GetIndex(int x, int y)
        {
            return y * Width + x;
        }
        
        public void FillRandom(int colorCount, int seed = 0)
        {
            System.Random rnd = new System.Random(seed);
            int idCounter = 1;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int color = rnd.Next(1, colorCount + 1);
                    SetBlock(x, y, new BlockData(idCounter++, x, y, color));
                }
            }
        }
    }
}
