namespace VoxDetroit.Core
{
    public static class VoxDetroitConstants
    {
        public const float VoxelSizeMeters = 0.5f;
        public const int ChunkSize = 32;
        public const int VoxelsPerChunk = ChunkSize * ChunkSize * ChunkSize;
        public const float ChunkWorldSizeMeters = ChunkSize * VoxelSizeMeters;
    }
}
