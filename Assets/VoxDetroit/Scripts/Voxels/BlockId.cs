namespace VoxDetroit.Voxels
{
    public enum BlockId : ushort
    {
        Air = 0,
        Soil = 1,
        Grass = 2,
        Concrete = 3,
        Asphalt = 4,
        Brick = 5,
        Glass = 6,
        Wood = 7,
        Water = 8,
        RoadMarking = 9
    }

    public static class BlockCatalog
    {
        // Prototype renderer uses one submesh/material slot per block ID.
        // Replace with a texture atlas/material batching strategy later.
        public const int MaterialSlotCount = 10;

        public static bool IsSolid(BlockId block)
        {
            return block != BlockId.Air &&
                   block != BlockId.Water;
        }

        public static bool OccludesFace(BlockId block)
        {
            return block != BlockId.Air &&
                   block != BlockId.Water &&
                   block != BlockId.Glass;
        }

        public static int GetMaterialSlot(BlockId block)
        {
            int slot = (int)block;

            return slot >= 0 && slot < MaterialSlotCount
                ? slot
                : 0;
        }
    }
}
