using System.Collections.Generic;
using UnityEngine;
using VoxDetroit.Core;
using VoxDetroit.World;

namespace VoxDetroit.Voxels
{
    public static class VoxelChunkMeshBuilder
    {
        private static readonly Vector3Int[] NeighborOffsets =
        {
            new Vector3Int(1, 0, 0),
            new Vector3Int(-1, 0, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, -1, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 0, -1)
        };

        private static readonly Vector3[,] FaceCorners =
        {
            { new Vector3(1,0,0), new Vector3(1,1,0), new Vector3(1,1,1), new Vector3(1,0,1) },
            { new Vector3(0,0,1), new Vector3(0,1,1), new Vector3(0,1,0), new Vector3(0,0,0) },
            { new Vector3(0,1,1), new Vector3(1,1,1), new Vector3(1,1,0), new Vector3(0,1,0) },
            { new Vector3(0,0,0), new Vector3(1,0,0), new Vector3(1,0,1), new Vector3(0,0,1) },
            { new Vector3(1,0,1), new Vector3(1,1,1), new Vector3(0,1,1), new Vector3(0,0,1) },
            { new Vector3(0,0,0), new Vector3(0,1,0), new Vector3(1,1,0), new Vector3(1,0,0) }
        };

        public static Mesh Build(VoxelChunkData chunk)
        {
            return Build(
                chunk,
                new ChunkCoord(0, 0, 0),
                null);
        }

        public static Mesh Build(
            VoxelChunkData chunk,
            ChunkCoord coord,
            IVoxelBlockSource neighborSource)
        {
            float scale = VoxDetroitConstants.VoxelSizeMeters;
            int size = VoxDetroitConstants.ChunkSize;

            var vertices = new List<Vector3>();
            var triangles = new List<int>();
            var normals = new List<Vector3>();
            var uvs = new List<Vector2>();

            for (int z = 0; z < size; z++)
            {
                for (int y = 0; y < size; y++)
                {
                    for (int x = 0; x < size; x++)
                    {
                        BlockId block = chunk.Get(x, y, z);
                        if (!BlockCatalog.IsSolid(block))
                        {
                            continue;
                        }

                        for (int face = 0; face < 6; face++)
                        {
                            if (FaceIsOccluded(
                                chunk,
                                coord,
                                neighborSource,
                                x,
                                y,
                                z,
                                face))
                            {
                                continue;
                            }

                            AddFace(
                                vertices,
                                triangles,
                                normals,
                                uvs,
                                x,
                                y,
                                z,
                                face,
                                scale);
                        }
                    }
                }
            }

            var mesh = new Mesh { name = $"Voxel Chunk {coord}" };

            if (vertices.Count > 65535)
            {
                mesh.indexFormat =
                    UnityEngine.Rendering.IndexFormat.UInt32;
            }

            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.SetNormals(normals);
            mesh.SetUVs(0, uvs);
            mesh.RecalculateBounds();

            return mesh;
        }

        private static bool FaceIsOccluded(
            VoxelChunkData chunk,
            ChunkCoord coord,
            IVoxelBlockSource neighborSource,
            int x,
            int y,
            int z,
            int face)
        {
            Vector3Int offset = NeighborOffsets[face];

            int nx = x + offset.x;
            int ny = y + offset.y;
            int nz = z + offset.z;

            if (chunk.TryGet(nx, ny, nz, out BlockId localNeighbor))
            {
                return BlockCatalog.OccludesFace(localNeighbor);
            }

            if (neighborSource == null)
            {
                return false;
            }

            int worldX = coord.WorldVoxelOriginX + x + offset.x;
            int worldY = coord.WorldVoxelOriginY + y + offset.y;
            int worldZ = coord.WorldVoxelOriginZ + z + offset.z;

            return neighborSource.TryGetBlock(
                       worldX,
                       worldY,
                       worldZ,
                       out BlockId worldNeighbor) &&
                   BlockCatalog.OccludesFace(worldNeighbor);
        }

        private static void AddFace(
            List<Vector3> vertices,
            List<int> triangles,
            List<Vector3> normals,
            List<Vector2> uvs,
            int x,
            int y,
            int z,
            int face,
            float scale)
        {
            int start = vertices.Count;
            Vector3 origin = new Vector3(
                x * scale,
                y * scale,
                z * scale);

            for (int i = 0; i < 4; i++)
            {
                vertices.Add(
                    origin + FaceCorners[face, i] * scale);
                normals.Add(NeighborOffsets[face]);
            }

            uvs.Add(new Vector2(0, 0));
            uvs.Add(new Vector2(0, 1));
            uvs.Add(new Vector2(1, 1));
            uvs.Add(new Vector2(1, 0));

            triangles.Add(start + 0);
            triangles.Add(start + 1);
            triangles.Add(start + 2);
            triangles.Add(start + 0);
            triangles.Add(start + 2);
            triangles.Add(start + 3);
        }
    }
}
