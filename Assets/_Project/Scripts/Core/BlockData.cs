using System;

namespace GemBlast.Core
{
    public readonly struct BlockData : IEquatable<BlockData>
    {
        public readonly int Id;
        public readonly int ColorIndex;
        public readonly int X;
        public readonly int Y;
        public readonly IconType Icon;

        public bool IsValid => Id > 0;

        public BlockData(int id, int x, int y, int colorIndex, IconType icon = IconType.Default)
        {
            Id = id;
            X = x;
            Y = y;
            ColorIndex = colorIndex;
            Icon = icon;
        }

        public BlockData WithPosition(int x, int y)
        {
            return new BlockData(Id, x, y, ColorIndex, Icon);
        }
        
        public BlockData WithIcon(IconType icon)
        {
            return new BlockData(Id, X, Y, ColorIndex, icon);
        }
        
        public override string ToString()
        {
            return $"[{X},{Y}] ID:{Id} Color:{ColorIndex} Icon:{Icon}";
        }

        public bool Equals(BlockData other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            return obj is BlockData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public static bool operator ==(BlockData left, BlockData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BlockData left, BlockData right)
        {
            return !left.Equals(right);
        }
    }
}
