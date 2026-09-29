using System;
using System.Collections.Generic;
using VoxDetroit.Core;
using VoxDetroit.Voxels;
using VoxDetroit.World;

namespace VoxDetroit.Detroit
{
    public static class DetroitFeatureRasterizer
    {
        // Guards against bad source data without clipping real Detroit towers.
        private const float MaxImportedBuildingHeightMeters = 300f;
        private const float GroundPaddingMeters = 8f;

        public static void Rasterize(
            DetroitImportDocument document,
            VoxelWorldData world)
        {
            if (document == null)
            {
                throw new ArgumentNullException(nameof(document));
            }

            if (world == null)
            {
                throw new ArgumentNullException(nameof(world));
            }

            RasterizePrototypeGround(
                document,
                world);

            if (document.roads != null)
            {
                foreach (RoadFeature road in document.roads)
                {
                    RasterizeRoad(road, world);
                }
            }

            if (document.buildings != null)
            {
                foreach (BuildingFeature building
                         in document.buildings)
                {
                    RasterizeBuilding(building, world);
                }
            }
        }

        public static void RasterizeRoad(
            RoadFeature road,
            VoxelWorldData world)
        {
            if (road == null ||
                road.centerline == null ||
                road.centerline.Length < 2)
            {
                return;
            }

            int radius = Math.Max(
                1,
                (int)Math.Ceiling(
                    Math.Max(road.widthMeters, 2f) /
                    VoxDetroitConstants.VoxelSizeMeters /
                    2f));

            for (int i = 0;
                 i < road.centerline.Length - 1;
                 i++)
            {
                GridPoint a =
                    ToGrid(road.centerline[i]);
                GridPoint b =
                    ToGrid(road.centerline[i + 1]);

                RasterizeLine(
                    a,
                    b,
                    (x, z) => PaintRoadDisc(
                        world,
                        x,
                        z,
                        radius));
            }
        }

        public static void RasterizeBuilding(
            BuildingFeature building,
            VoxelWorldData world)
        {
            if (building == null ||
                building.footprint == null ||
                building.footprint.Length < 3)
            {
                return;
            }

            var polygon =
                new List<GridPoint>(
                    building.footprint.Length);

            foreach (GeoPoint point in building.footprint)
            {
                polygon.Add(ToGrid(point));
            }

            int minX = int.MaxValue;
            int maxX = int.MinValue;
            int minZ = int.MaxValue;
            int maxZ = int.MinValue;

            foreach (GridPoint point in polygon)
            {
                minX = Math.Min(minX, point.X);
                maxX = Math.Max(maxX, point.X);
                minZ = Math.Min(minZ, point.Z);
                maxZ = Math.Max(maxZ, point.Z);
            }

            float heightMeters = Math.Min(
                ResolveHeight(building),
                MaxImportedBuildingHeightMeters);

            int heightVoxels = Math.Max(
                2,
                (int)Math.Ceiling(
                    heightMeters /
                    VoxDetroitConstants.VoxelSizeMeters));

            for (int z = minZ; z <= maxZ; z++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    if (!PointInPolygon(
                            x + 0.5,
                            z + 0.5,
                            polygon))
                    {
                        continue;
                    }

                    world.SetBlock(
                        x,
                        0,
                        z,
                        BlockId.Concrete);

                    bool edge =
                        !PointInPolygon(
                            x - 0.5,
                            z + 0.5,
                            polygon) ||
                        !PointInPolygon(
                            x + 1.5,
                            z + 0.5,
                            polygon) ||
                        !PointInPolygon(
                            x + 0.5,
                            z - 0.5,
                            polygon) ||
                        !PointInPolygon(
                            x + 0.5,
                            z + 1.5,
                            polygon);

                    for (int y = 1;
                         y <= heightVoxels;
                         y++)
                    {
                        if (edge)
                        {
                            world.SetBlock(
                                x,
                                y,
                                z,
                                BlockId.Brick);
                        }
                        else if (y == heightVoxels)
                        {
                            world.SetBlock(
                                x,
                                y,
                                z,
                                BlockId.Concrete);
                        }
                    }
                }
            }
        }

        private static void RasterizePrototypeGround(
            DetroitImportDocument document,
            VoxelWorldData world)
        {
            if (!TryResolveDocumentExtents(
                    document,
                    out int minX,
                    out int maxX,
                    out int minZ,
                    out int maxZ))
            {
                return;
            }

            int paddingVoxels =
                Math.Max(
                    1,
                    (int)Math.Ceiling(
                        GroundPaddingMeters /
                        VoxDetroitConstants.VoxelSizeMeters));

            minX -= paddingVoxels;
            maxX += paddingVoxels;
            minZ -= paddingVoxels;
            maxZ += paddingVoxels;

            for (int z = minZ; z <= maxZ; z++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    world.SetBlock(
                        x,
                        0,
                        z,
                        BlockId.Concrete);
                }
            }
        }

        private static bool TryResolveDocumentExtents(
            DetroitImportDocument document,
            out int minX,
            out int maxX,
            out int minZ,
            out int maxZ)
        {
            minX = int.MaxValue;
            maxX = int.MinValue;
            minZ = int.MaxValue;
            maxZ = int.MinValue;
            bool any = false;

            if (document.roads != null)
            {
                foreach (RoadFeature road in document.roads)
                {
                    if (road?.centerline == null)
                    {
                        continue;
                    }

                    foreach (GeoPoint point in road.centerline)
                    {
                        ExpandBounds(
                            point,
                            ref minX,
                            ref maxX,
                            ref minZ,
                            ref maxZ,
                            ref any);
                    }
                }
            }

            if (document.buildings != null)
            {
                foreach (BuildingFeature building
                         in document.buildings)
                {
                    if (building?.footprint == null)
                    {
                        continue;
                    }

                    foreach (GeoPoint point
                             in building.footprint)
                    {
                        ExpandBounds(
                            point,
                            ref minX,
                            ref maxX,
                            ref minZ,
                            ref maxZ,
                            ref any);
                    }
                }
            }

            if (!any &&
                document.bounds != null &&
                document.bounds.north > document.bounds.south &&
                document.bounds.east > document.bounds.west)
            {
                ExpandBounds(
                    new GeoPoint(
                        document.bounds.south,
                        document.bounds.west),
                    ref minX,
                    ref maxX,
                    ref minZ,
                    ref maxZ,
                    ref any);

                ExpandBounds(
                    new GeoPoint(
                        document.bounds.north,
                        document.bounds.east),
                    ref minX,
                    ref maxX,
                    ref minZ,
                    ref maxZ,
                    ref any);
            }

            return any;
        }

        private static void ExpandBounds(
            GeoPoint point,
            ref int minX,
            ref int maxX,
            ref int minZ,
            ref int maxZ,
            ref bool any)
        {
            if (point == null)
            {
                return;
            }

            GridPoint grid =
                ToGrid(point);

            minX = Math.Min(minX, grid.X);
            maxX = Math.Max(maxX, grid.X);
            minZ = Math.Min(minZ, grid.Z);
            maxZ = Math.Max(maxZ, grid.Z);
            any = true;
        }

        private static void PaintRoadDisc(
            VoxelWorldData world,
            int centerX,
            int centerZ,
            int radius)
        {
            int radiusSq = radius * radius;

            for (int dz = -radius; dz <= radius; dz++)
            {
                for (int dx = -radius; dx <= radius; dx++)
                {
                    if ((dx * dx) + (dz * dz) >
                        radiusSq)
                    {
                        continue;
                    }

                    world.SetBlock(
                        centerX + dx,
                        0,
                        centerZ + dz,
                        BlockId.Asphalt);
                }
            }
        }

        private static void RasterizeLine(
            GridPoint a,
            GridPoint b,
            Action<int, int> plot)
        {
            int x0 = a.X;
            int z0 = a.Z;
            int x1 = b.X;
            int z1 = b.Z;

            int dx = Math.Abs(x1 - x0);
            int sx = x0 < x1 ? 1 : -1;
            int dz = -Math.Abs(z1 - z0);
            int sz = z0 < z1 ? 1 : -1;
            int error = dx + dz;

            while (true)
            {
                plot(x0, z0);

                if (x0 == x1 && z0 == z1)
                {
                    break;
                }

                int e2 = 2 * error;

                if (e2 >= dz)
                {
                    error += dz;
                    x0 += sx;
                }

                if (e2 <= dx)
                {
                    error += dx;
                    z0 += sz;
                }
            }
        }

        private static GridPoint ToGrid(GeoPoint point)
        {
            (int x, int z) =
                DetroitGeoReference.ToWorldVoxel(
                    point.latitude,
                    point.longitude,
                    VoxDetroitConstants.VoxelSizeMeters);

            return new GridPoint(x, z);
        }

        private static float ResolveHeight(
            BuildingFeature building)
        {
            if (building.heightMeters > 0f)
            {
                return building.heightMeters;
            }

            if (building.levels > 0)
            {
                return building.levels * 3.2f;
            }

            switch (building.buildingType)
            {
                case "canopy":
                    return 4f;

                case "garage":
                case "garages":
                    return 4.5f;

                case "house":
                case "detached":
                case "residential":
                    return 6.4f;

                case "construction":
                    return 6f;

                default:
                    return 9.6f;
            }
        }

        private static bool PointInPolygon(
            double x,
            double z,
            List<GridPoint> polygon)
        {
            bool inside = false;

            for (int i = 0, j = polygon.Count - 1;
                 i < polygon.Count;
                 j = i++)
            {
                GridPoint pi = polygon[i];
                GridPoint pj = polygon[j];

                bool crosses =
                    ((pi.Z > z) != (pj.Z > z)) &&
                    (x <
                     (double)(pj.X - pi.X) *
                     (z - pi.Z) /
                     (pj.Z - pi.Z) +
                     pi.X);

                if (crosses)
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        private readonly struct GridPoint
        {
            public readonly int X;
            public readonly int Z;

            public GridPoint(int x, int z)
            {
                X = x;
                Z = z;
            }
        }
    }
}
