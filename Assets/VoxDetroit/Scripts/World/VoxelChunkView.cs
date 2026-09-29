using UnityEngine;
using VoxDetroit.Voxels;

namespace VoxDetroit.World
{
    [RequireComponent(
        typeof(MeshFilter),
        typeof(MeshRenderer),
        typeof(MeshCollider))]
    public sealed class VoxelChunkView : MonoBehaviour
    {
        public ChunkCoord Coord { get; private set; }
        public VoxelChunkData Data { get; private set; }

        private MeshFilter _meshFilter;
        private MeshCollider _meshCollider;
        private Mesh _mesh;
        private IVoxelBlockSource _neighborSource;

        private void Awake()
        {
            _meshFilter = GetComponent<MeshFilter>();
            _meshCollider = GetComponent<MeshCollider>();
        }

        public void Initialize(
            ChunkCoord coord,
            VoxelChunkData data,
            IVoxelBlockSource neighborSource = null)
        {
            Coord = coord;
            Data = data;
            _neighborSource = neighborSource;
            RebuildMesh();
        }

        public void RebuildMesh()
        {
            if (Data == null)
            {
                return;
            }

            if (_mesh != null)
            {
                Destroy(_mesh);
            }

            _mesh = VoxelChunkMeshBuilder.Build(
                Data,
                Coord,
                _neighborSource);

            _meshFilter.sharedMesh = _mesh;
            _meshCollider.sharedMesh = null;
            _meshCollider.sharedMesh = _mesh;
        }

        private void OnDestroy()
        {
            if (_mesh != null)
            {
                Destroy(_mesh);
            }
        }
    }
}
