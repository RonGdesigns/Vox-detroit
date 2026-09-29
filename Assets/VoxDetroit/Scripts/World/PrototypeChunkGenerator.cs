using VoxDetroit.Core;
using VoxDetroit.Voxels;

namespace VoxDetroit.World
{
    public static class PrototypeChunkGenerator
    {
        private const int RoadPeriodVoxels = 64;
        private const int RoadWidthVoxels = 10;

        public static VoxelChunkData Generate(ChunkCoord coord)
        {
            var data = new VoxelChunkData();

            if (coord.Y != 0)
            {
                return data;
            }

            int size = VoxDetroitConstants.ChunkSize;

            for (int z = 0; z < size; z++)
            {
                int worldZ = coord.WorldVoxelOriginZ + z;

                for (int x = 0; x < size; x++)
                {
                    int worldX = coord.WorldVoxelOriginX + x;
                    bool road = IsRoad(worldX) || IsRoad(worldZ);

                    data.Set(
                        x,
                        0,
                        z,
                        road ? BlockId.Asphalt : BlockId.Grass);

                    if (!road && IsPrototypeBuilding(worldX, worldZ))
                    {
                        AddBuildingColumn(data, x, z);
                    }
                }
            }

            return data;
        }

        private static void AddBuildingColumn(
            VoxelChunkData data,
            int x,
            int z)
        {
            int height = 8;
            int worldXInCell = PositiveMod(
                x,
                VoxDetroitConstants.ChunkSize);
            int worldZInCell = PositiveMod(
                z,
                VoxDetroitConstants.ChunkSize);

            bool localEdge =
                worldXInCell == 6 ||
                worldXInCell == 25 ||
                worldZInCell == 6 ||
                worldZInCell == 25;

            for (int y = 1; y <= height; y++)
            {
                if (localEdge || y == height)
                {
                    data.Set(x, y, z, BlockId.Brick);
                }
            }
        }

        private static bool IsPrototypeBuilding(int worldX, int worldZ)
        {
            int x = PositiveMod(worldX, RoadPeriodVoxels);
            int z = PositiveMod(worldZ, RoadPeriodVoxels);

            return x >= 16 && x <= 47 &&
                   z >= 16 && z <= 47;
        }

        private static bool IsRoad(int worldVoxel)
        {
            return PositiveMod(
                worldVoxel,
                RoadPeriodVoxels) < RoadWidthVoxels;
        }

        private static int PositiveMod(int value, int divisor)
        {
            int result = value % divisor;
            return result < 0 ? result + divisor : result;
        }
    }
}
