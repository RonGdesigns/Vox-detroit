using UnityEngine;
using VoxDetroit.Core;
using VoxDetroit.Voxels;

namespace VoxDetroit.World
{
    public sealed class VoxelTestBootstrap : MonoBehaviour
    {
        [SerializeField] private Material voxelMaterial;

        private void Start()
        {
            CreateTestChunk(new ChunkCoord(0, 0, 0));
        }

        private void CreateTestChunk(ChunkCoord coord)
        {
            var data = new VoxelChunkData();
            int size = VoxDetroitConstants.ChunkSize;

            for (int z = 0; z < size; z++)
            {
                for (int x = 0; x < size; x++)
                {
                    data.Set(x, 0, z, BlockId.Concrete);

                    if ((x >= 4 && x <= 10) && (z >= 4 && z <= 10))
                    {
                        for (int y = 1; y <= 6; y++)
                        {
                            bool shell = x == 4 || x == 10 || z == 4 || z == 10 || y == 6;
                            if (shell)
                            {
                                data.Set(x, y, z, BlockId.Brick);
                            }
                        }
                    }
                }
            }

            var chunkObject = new GameObject($"Chunk {coord}");
            chunkObject.transform.SetParent(transform, false);
            chunkObject.transform.localPosition = new Vector3(
                coord.X * VoxDetroitConstants.ChunkWorldSizeMeters,
                coord.Y * VoxDetroitConstants.ChunkWorldSizeMeters,
                coord.Z * VoxDetroitConstants.ChunkWorldSizeMeters);

            var view = chunkObject.AddComponent<VoxelChunkView>();
            var renderer = chunkObject.GetComponent<MeshRenderer>();
            renderer.sharedMaterial = ResolveMaterial();
            view.Initialize(coord, data);
        }

        private Material ResolveMaterial()
        {
            if (voxelMaterial != null)
            {
                return voxelMaterial;
            }

            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            var material = new Material(shader) { name = "Runtime Voxel Test Material" };
            material.color = new Color(0.55f, 0.55f, 0.58f);
            return material;
        }
    }
}
