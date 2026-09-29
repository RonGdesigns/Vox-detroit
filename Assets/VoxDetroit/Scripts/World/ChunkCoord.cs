using System;
using VoxDetroit.Core;

namespace VoxDetroit.World
{
    [Serializable]
    public readonly struct ChunkCoord : IEquatable<ChunkCoord>
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Z;

        public ChunkCoord(int x, int y, int z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static ChunkCoord FromWorldVoxel(int x, int y, int z)
        {
            int size = VoxDetroitConstants.ChunkSize;
            return new ChunkCoord(
                FloorDiv(x, size),
                FloorDiv(y, size),
                FloorDiv(z, size));
        }

        public static int ToLocalVoxel(int worldVoxel)
        {
            return PositiveMod(worldVoxel, VoxDetroitConstants.ChunkSize);
        }

        public bool Equals(ChunkCoord other)
        {
            return X == other.X && Y == other.Y && Z == other.Z;
        }

        public override bool Equals(object obj)
        {
            return obj is ChunkCoord other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = X;
                hash = (hash * 397) ^ Y;
                hash = (hash * 397) ^ Z;
                return hash;
            }
        }

        public override string ToString()
        {
            return $"({X}, {Y}, {Z})";
        }

        private static int FloorDiv(int value, int divisor)
        {
            int quotient = value / divisor;
            int remainder = value % divisor;
            if (remainder != 0 && ((remainder < 0) != (divisor < 0)))
            {
                quotient--;
            }
            return quotient;
        }

        private static int PositiveMod(int value, int divisor)
        {
            int result = value % divisor;
            return result < 0 ? result + divisor : result;
        }
    }
}
