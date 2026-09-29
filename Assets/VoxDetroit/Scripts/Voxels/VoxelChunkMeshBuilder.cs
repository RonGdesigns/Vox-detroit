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

        private static readonly Vector3Int[] FaceU =
        {
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 0, 1),
            new Vector3Int(1, 0, 0),
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, 1, 0)
        };

        private static readonly Vector3Int[] FaceV =
        {
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 1, 0),
            new Vector3Int(1, 0, 0),
            new Vector3Int(0, 0, 1),
            new Vector3Int(0, 1, 0),
            new Vector3Int(1, 0, 0)
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
            float scale =
                VoxDetroitConstants.VoxelSizeMeters;

            int size =
                VoxDetroitConstants.ChunkSize;

            var vertices =
                new List<Vector3>();

            var normals =
                new List<Vector3>();

            var uvs =
                new List<Vector2>();

            List<int>[] trianglesBySlot =
                CreateTriangleLists();

            var mask =
                new BlockId[size * size];

            for (int face = 0; face < 6; face++)
            {
                for (int slice = 0;
                     slice < size;
                     slice++)
                {
                    FillFaceMask(
                        mask,
                        chunk,
                        coord,
                        neighborSource,
                        face,
                        slice,
                        size);

                    EmitGreedyQuads(
                        mask,
                        face,
                        slice,
                        size,
                        scale,
                        vertices,
                        normals,
                        uvs,
                        trianglesBySlot);
                }
            }

            var mesh =
                new Mesh
                {
                    name =
                        $"Voxel Chunk {coord} Greedy"
                };

            if (vertices.Count > 65535)
            {
                mesh.indexFormat =
                    UnityEngine.Rendering.IndexFormat.UInt32;
            }

            mesh.SetVertices(vertices);
            mesh.SetNormals(normals);
            mesh.SetUVs(0, uvs);

            mesh.subMeshCount =
                BlockCatalog.MaterialSlotCount;

            for (int slot = 0;
                 slot < trianglesBySlot.Length;
                 slot++)
            {
                mesh.SetTriangles(
                    trianglesBySlot[slot],
                    slot,
                    false);
            }

            mesh.RecalculateBounds();
            return mesh;
        }

        private static void FillFaceMask(
            BlockId[] mask,
            VoxelChunkData chunk,
            ChunkCoord coord,
            IVoxelBlockSource neighborSource,
            int face,
            int slice,
            int size)
        {
            int index = 0;

            for (int v = 0; v < size; v++)
            {
                for (int u = 0; u < size; u++)
                {
                    GetVoxelCoordinates(
                        face,
                        slice,
                        u,
                        v,
                        out int x,
                        out int y,
                        out int z);

                    BlockId block =
                        chunk.Get(x, y, z);

                    mask[index++] =
                        BlockCatalog.IsSolid(block) &&
                        !FaceIsOccluded(
                            chunk,
                            coord,
                            neighborSource,
                            x,
                            y,
                            z,
                            face)
                            ? block
                            : BlockId.Air;
                }
            }
        }

        private static void EmitGreedyQuads(
            BlockId[] mask,
            int face,
            int slice,
            int size,
            float scale,
            List<Vector3> vertices,
            List<Vector3> normals,
            List<Vector2> uvs,
            List<int>[] trianglesBySlot)
        {
            for (int v = 0; v < size; v++)
            {
                int u = 0;

                while (u < size)
                {
                    int index =
                        u + (v * size);

                    BlockId block =
                        mask[index];

                    if (block == BlockId.Air)
                    {
                        u++;
                        continue;
                    }

                    int width = 1;

                    while (u + width < size &&
                           mask[index + width] == block)
                    {
                        width++;
                    }

                    int height = 1;
                    bool canGrow = true;

                    while (v + height < size &&
                           canGrow)
                    {
                        int row =
                            (v + height) * size;

                        for (int x = 0;
                             x < width;
                             x++)
                        {
                            if (mask[
                                    row +
                                    u +
                                    x] != block)
                            {
                                canGrow = false;
                                break;
                            }
                        }

                        if (canGrow)
                        {
                            height++;
                        }
                    }

                    int materialSlot =
                        BlockCatalog.GetMaterialSlot(
                            block);

                    AddMergedFace(
                        vertices,
                        trianglesBySlot[
                            materialSlot],
                        normals,
                        uvs,
                        face,
                        slice,
                        u,
                        v,
                        width,
                        height,
                        scale);

                    for (int clearV = 0;
                         clearV < height;
                         clearV++)
                    {
                        int row =
                            (v + clearV) * size;

                        for (int clearU = 0;
                             clearU < width;
                             clearU++)
                        {
                            mask[
                                row +
                                u +
                                clearU] =
                                BlockId.Air;
                        }
                    }

                    u += width;
                }
            }
        }

        private static void AddMergedFace(
            List<Vector3> vertices,
            List<int> triangles,
            List<Vector3> normals,
            List<Vector2> uvs,
            int face,
            int slice,
            int u,
            int v,
            int width,
            int height,
            float scale)
        {
            int start =
                vertices.Count;

            Vector3 origin =
                GetFaceOrigin(
                    face,
                    slice,
                    u,
                    v) *
                scale;

            Vector3 edgeU =
                (Vector3)FaceU[face] *
                width *
                scale;

            Vector3 edgeV =
                (Vector3)FaceV[face] *
                height *
                scale;

            vertices.Add(origin);
            vertices.Add(origin + edgeU);
            vertices.Add(origin + edgeU + edgeV);
            vertices.Add(origin + edgeV);

            Vector3 normal =
                NeighborOffsets[face];

            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);
            normals.Add(normal);

            uvs.Add(new Vector2(0f, 0f));
            uvs.Add(new Vector2(width, 0f));
            uvs.Add(new Vector2(width, height));
            uvs.Add(new Vector2(0f, height));

            triangles.Add(start + 0);
            triangles.Add(start + 1);
            triangles.Add(start + 2);
            triangles.Add(start + 0);
            triangles.Add(start + 2);
            triangles.Add(start + 3);
        }

        private static Vector3 GetFaceOrigin(
            int face,
            int slice,
            int u,
            int v)
        {
            switch (face)
            {
                case 0:
                    return new Vector3(
                        slice + 1,
                        u,
                        v);

                case 1:
                    return new Vector3(
                        slice,
                        v,
                        u);

                case 2:
                    return new Vector3(
                        v,
                        slice + 1,
                        u);

                case 3:
                    return new Vector3(
                        u,
                        slice,
                        v);

                case 4:
                    return new Vector3(
                        u,
                        v,
                        slice + 1);

                default:
                    return new Vector3(
                        v,
                        u,
                        slice);
            }
        }

        private static void GetVoxelCoordinates(
            int face,
            int slice,
            int u,
            int v,
            out int x,
            out int y,
            out int z)
        {
            switch (face)
            {
                case 0:
                    x = slice;
                    y = u;
                    z = v;
                    return;

                case 1:
                    x = slice;
                    y = v;
                    z = u;
                    return;

                case 2:
                    x = v;
                    y = slice;
                    z = u;
                    return;

                case 3:
                    x = u;
                    y = slice;
                    z = v;
                    return;

                case 4:
                    x = u;
                    y = v;
                    z = slice;
                    return;

                default:
                    x = v;
                    y = u;
                    z = slice;
                    return;
            }
        }

        private static List<int>[] CreateTriangleLists()
        {
            var lists =
                new List<int>[
                    BlockCatalog.MaterialSlotCount];

            for (int i = 0;
                 i < lists.Length;
                 i++)
            {
                lists[i] =
                    new List<int>();
            }

            return lists;
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
            Vector3Int offset =
                NeighborOffsets[face];

            int nx = x + offset.x;
            int ny = y + offset.y;
            int nz = z + offset.z;

            if (chunk.TryGet(
                    nx,
                    ny,
                    nz,
                    out BlockId localNeighbor))
            {
                return
                    BlockCatalog.OccludesFace(
                        localNeighbor);
            }

            if (neighborSource == null)
            {
                return false;
            }

            int worldX =
                coord.WorldVoxelOriginX +
                x +
                offset.x;

            int worldY =
                coord.WorldVoxelOriginY +
                y +
                offset.y;

            int worldZ =
                coord.WorldVoxelOriginZ +
                z +
                offset.z;

            return
                neighborSource.TryGetBlock(
                    worldX,
                    worldY,
                    worldZ,
                    out BlockId worldNeighbor) &&
                BlockCatalog.OccludesFace(
                    worldNeighbor);
        }
    }
}
