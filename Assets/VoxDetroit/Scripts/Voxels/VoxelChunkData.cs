using System;
using VoxDetroit.Core;

namespace VoxDetroit.Voxels
{
    [Serializable]
    public sealed class VoxelChunkData
    {
        private readonly BlockId[] _blocks;

        public VoxelChunkData()
        {
            _blocks = new BlockId[VoxDetroitConstants.VoxelsPerChunk];
        }

        public BlockId Get(int x, int y, int z)
        {
            ValidateLocalCoordinate(x, y, z);
            return _blocks[ToIndex(x, y, z)];
        }

        public bool TryGet(int x, int y, int z, out BlockId block)
        {
            if (!IsInside(x, y, z))
            {
                block = BlockId.Air;
                return false;
            }

            block = _blocks[ToIndex(x, y, z)];
            return true;
        }

        public void Set(int x, int y, int z, BlockId block)
        {
            ValidateLocalCoordinate(x, y, z);
            _blocks[ToIndex(x, y, z)] = block;
        }

        public void Fill(BlockId block)
        {
            Array.Fill(_blocks, block);
        }

        public static bool IsInside(int x, int y, int z)
        {
            int size = VoxDetroitConstants.ChunkSize;
            return x >= 0 && x < size &&
                   y >= 0 && y < size &&
                   z >= 0 && z < size;
        }

        private static int ToIndex(int x, int y, int z)
        {
            int size = VoxDetroitConstants.ChunkSize;
            return x + size * (y + size * z);
        }

        private static void ValidateLocalCoordinate(int x, int y, int z)
        {
            if (!IsInside(x, y, z))
            {
                throw new ArgumentOutOfRangeException(
                    $"Voxel coordinate ({x}, {y}, {z}) is outside a {VoxDetroitConstants.ChunkSize}³ chunk.");
            }
        }
    }
}
