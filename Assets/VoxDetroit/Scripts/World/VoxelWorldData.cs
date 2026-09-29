using System;
using System.Collections.Generic;
using VoxDetroit.Voxels;

namespace VoxDetroit.World
{
    public sealed class VoxelWorldData : IVoxelBlockSource
    {
        private readonly Dictionary<ChunkCoord, VoxelChunkData> _chunks =
            new Dictionary<ChunkCoord, VoxelChunkData>();

        public int ChunkCount => _chunks.Count;
        public IEnumerable<ChunkCoord> ChunkCoords => _chunks.Keys;

        public bool TryGetChunk(ChunkCoord coord, out VoxelChunkData chunk)
        {
            return _chunks.TryGetValue(coord, out chunk);
        }

        public VoxelChunkData GetOrCreateChunk(
            ChunkCoord coord,
            Func<ChunkCoord, VoxelChunkData> factory = null)
        {
            if (_chunks.TryGetValue(coord, out VoxelChunkData existing))
            {
                return existing;
            }

            VoxelChunkData created = factory != null
                ? factory(coord)
                : new VoxelChunkData();

            if (created == null)
            {
                throw new InvalidOperationException(
                    $"Chunk factory returned null for {coord}.");
            }

            _chunks.Add(coord, created);
            return created;
        }

        public void SetChunk(ChunkCoord coord, VoxelChunkData chunk)
        {
            if (chunk == null)
            {
                throw new ArgumentNullException(nameof(chunk));
            }

            _chunks[coord] = chunk;
        }

        public bool RemoveChunk(ChunkCoord coord)
        {
            return _chunks.Remove(coord);
        }

        public bool TryGetBlock(
            int worldX,
            int worldY,
            int worldZ,
            out BlockId block)
        {
            ChunkCoord coord = ChunkCoord.FromWorldVoxel(
                worldX,
                worldY,
                worldZ);

            if (!_chunks.TryGetValue(coord, out VoxelChunkData chunk))
            {
                block = BlockId.Air;
                return false;
            }

            int localX = ChunkCoord.ToLocalVoxel(worldX);
            int localY = ChunkCoord.ToLocalVoxel(worldY);
            int localZ = ChunkCoord.ToLocalVoxel(worldZ);

            block = chunk.Get(localX, localY, localZ);
            return true;
        }

        public BlockId GetBlockOrAir(int worldX, int worldY, int worldZ)
        {
            return TryGetBlock(worldX, worldY, worldZ, out BlockId block)
                ? block
                : BlockId.Air;
        }

        public void SetBlock(
            int worldX,
            int worldY,
            int worldZ,
            BlockId block)
        {
            ChunkCoord coord = ChunkCoord.FromWorldVoxel(
                worldX,
                worldY,
                worldZ);

            VoxelChunkData chunk = GetOrCreateChunk(coord);
            chunk.Set(
                ChunkCoord.ToLocalVoxel(worldX),
                ChunkCoord.ToLocalVoxel(worldY),
                ChunkCoord.ToLocalVoxel(worldZ),
                block);
        }

        public void Clear()
        {
            _chunks.Clear();
        }
    }
}
