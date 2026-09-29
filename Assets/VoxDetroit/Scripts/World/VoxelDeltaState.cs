using System;
using System.Collections.Generic;
using VoxDetroit.Voxels;

namespace VoxDetroit.World
{
    [Serializable]
    public sealed class VoxelDeltaRecord
    {
        public int x;
        public int y;
        public int z;
        public ushort blockId;
    }

    [Serializable]
    public sealed class VoxelDeltaState
    {
        public List<VoxelDeltaRecord> changes =
            new List<VoxelDeltaRecord>();
    }

    public sealed class VoxelDeltaService
    {
        private readonly VoxelDeltaState _state;

        public VoxelDeltaService(VoxelDeltaState state)
        {
            _state = state ??
                throw new ArgumentNullException(nameof(state));
        }

        public void Record(
            int worldX,
            int worldY,
            int worldZ,
            BlockId block)
        {
            VoxelDeltaRecord existing =
                Find(worldX, worldY, worldZ);

            if (existing == null)
            {
                _state.changes.Add(
                    new VoxelDeltaRecord
                    {
                        x = worldX,
                        y = worldY,
                        z = worldZ,
                        blockId = (ushort)block
                    });

                return;
            }

            existing.blockId = (ushort)block;
        }

        public void ApplyTo(VoxelWorldData world)
        {
            if (world == null)
            {
                throw new ArgumentNullException(nameof(world));
            }

            foreach (VoxelDeltaRecord change in _state.changes)
            {
                if (change == null)
                {
                    continue;
                }

                world.SetBlock(
                    change.x,
                    change.y,
                    change.z,
                    (BlockId)change.blockId);
            }
        }

        private VoxelDeltaRecord Find(
            int x,
            int y,
            int z)
        {
            foreach (VoxelDeltaRecord change in _state.changes)
            {
                if (change != null &&
                    change.x == x &&
                    change.y == y &&
                    change.z == z)
                {
                    return change;
                }
            }

            return null;
        }
    }
}
