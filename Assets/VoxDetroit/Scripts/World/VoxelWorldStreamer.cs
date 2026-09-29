using System.Collections.Generic;
using UnityEngine;
using VoxDetroit.Core;
using VoxDetroit.Voxels;

namespace VoxDetroit.World
{
    public sealed class VoxelWorldStreamer : MonoBehaviour
    {
        [Header("Streaming")]
        [SerializeField] private Transform focus;
        [SerializeField, Range(0, 8)] private int renderRadius = 1;
        [SerializeField, Min(0)] private int chunksBelowFocus = 0;
        [SerializeField, Min(0)] private int chunksAboveFocus = 4;
        [SerializeField] private bool generatePrototypeChunks = true;

        [Header("Rendering")]
        [SerializeField] private Material voxelMaterial;

        private readonly Dictionary<ChunkCoord, VoxelChunkView> _views =
            new Dictionary<ChunkCoord, VoxelChunkView>();

        private readonly HashSet<ChunkCoord> _residentCoords =
            new HashSet<ChunkCoord>();

        private readonly VoxelWorldData _world =
            new VoxelWorldData();

        private ResidentBlockSource _residentBlockSource;
        private Material _runtimeMaterial;
        private ChunkCoord? _lastCenter;

        public VoxelWorldData World => _world;
        public int ResidentViewCount => _views.Count;

        public bool GeneratePrototypeChunks
        {
            get => generatePrototypeChunks;
            set => generatePrototypeChunks = value;
        }

        private void Awake()
        {
            EnsureInitialized();
        }

        private void Start()
        {
            if (focus == null && Camera.main != null)
            {
                focus = Camera.main.transform;
            }

            ForceRefresh();
        }

        private void Update()
        {
            if (focus == null)
            {
                return;
            }

            ChunkCoord center = GetFocusChunk();

            if (!_lastCenter.HasValue ||
                !_lastCenter.Value.Equals(center))
            {
                Refresh(center);
            }
        }

        public void SetFocus(Transform newFocus)
        {
            focus = newFocus;
            ForceRefresh();
        }

        public void ForceRefresh()
        {
            EnsureInitialized();

            ChunkCoord center = focus != null
                ? GetFocusChunk()
                : new ChunkCoord(0, 0, 0);

            Refresh(center);
        }

        private void EnsureInitialized()
        {
            if (_residentBlockSource == null)
            {
                _residentBlockSource =
                    new ResidentBlockSource(
                        _world,
                        _residentCoords);
            }
        }

        private ChunkCoord GetFocusChunk()
        {
            Vector3 local =
                transform.InverseTransformPoint(focus.position);

            return ChunkCoord.FromWorldMeters(
                local.x,
                local.y,
                local.z);
        }

        private void Refresh(ChunkCoord center)
        {
            EnsureInitialized();
            _lastCenter = center;

            HashSet<ChunkCoord> target =
                BuildTargetSet(center);

            _residentCoords.Clear();
            foreach (ChunkCoord coord in target)
            {
                _residentCoords.Add(coord);
            }

            RemoveViewsOutside(target);

            foreach (ChunkCoord coord in target)
            {
                if (!_world.TryGetChunk(
                        coord,
                        out VoxelChunkData data))
                {
                    if (!generatePrototypeChunks)
                    {
                        continue;
                    }

                    data = _world.GetOrCreateChunk(
                        coord,
                        PrototypeChunkGenerator.Generate);
                }

                if (!_views.ContainsKey(coord))
                {
                    _views.Add(
                        coord,
                        CreateView(coord, data));
                }
            }

            foreach (VoxelChunkView view in _views.Values)
            {
                view.RebuildMesh();
            }
        }

        private HashSet<ChunkCoord> BuildTargetSet(
            ChunkCoord center)
        {
            var target = new HashSet<ChunkCoord>();

            // Imported city data already knows which vertical chunks
            // contain geometry. Load every populated Y chunk inside
            // the horizontal render radius so tall buildings are not
            // clipped by a fixed height window.
            if (!generatePrototypeChunks &&
                _world.ChunkCount > 0)
            {
                foreach (ChunkCoord coord in _world.ChunkCoords)
                {
                    if (System.Math.Abs(coord.X - center.X) <=
                            renderRadius &&
                        System.Math.Abs(coord.Z - center.Z) <=
                            renderRadius)
                    {
                        target.Add(coord);
                    }
                }

                return target;
            }

            int minY = center.Y - chunksBelowFocus;
            int maxY = center.Y + chunksAboveFocus;

            for (int y = minY; y <= maxY; y++)
            {
                for (int z = -renderRadius;
                     z <= renderRadius;
                     z++)
                {
                    for (int x = -renderRadius;
                         x <= renderRadius;
                         x++)
                    {
                        target.Add(
                            new ChunkCoord(
                                center.X + x,
                                y,
                                center.Z + z));
                    }
                }
            }

            return target;
        }

        private void RemoveViewsOutside(
            HashSet<ChunkCoord> target)
        {
            var remove = new List<ChunkCoord>();

            foreach (
                KeyValuePair<ChunkCoord, VoxelChunkView> pair
                in _views)
            {
                if (!target.Contains(pair.Key))
                {
                    remove.Add(pair.Key);
                }
            }

            foreach (ChunkCoord coord in remove)
            {
                VoxelChunkView view = _views[coord];

                if (view != null)
                {
                    Destroy(view.gameObject);
                }

                _views.Remove(coord);
            }
        }

        private VoxelChunkView CreateView(
            ChunkCoord coord,
            VoxelChunkData data)
        {
            var chunkObject =
                new GameObject($"Chunk {coord}");

            chunkObject.transform.SetParent(
                transform,
                false);

            chunkObject.transform.localPosition =
                new Vector3(
                    coord.X *
                    VoxDetroitConstants.ChunkWorldSizeMeters,
                    coord.Y *
                    VoxDetroitConstants.ChunkWorldSizeMeters,
                    coord.Z *
                    VoxDetroitConstants.ChunkWorldSizeMeters);

            VoxelChunkView view =
                chunkObject.AddComponent<VoxelChunkView>();

            MeshRenderer renderer =
                chunkObject.GetComponent<MeshRenderer>();

            renderer.sharedMaterial = ResolveMaterial();

            view.Initialize(
                coord,
                data,
                _residentBlockSource);

            return view;
        }

        private Material ResolveMaterial()
        {
            if (voxelMaterial != null)
            {
                return voxelMaterial;
            }

            if (_runtimeMaterial != null)
            {
                return _runtimeMaterial;
            }

            Shader shader =
                Shader.Find(
                    "Universal Render Pipeline/Lit");

            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            if (shader == null)
            {
                Debug.LogError(
                    "Vox Detroit could not find a default shader.");
                return null;
            }

            _runtimeMaterial = new Material(shader)
            {
                name = "Runtime Voxel Material",
                color = new Color(0.55f, 0.55f, 0.58f)
            };

            return _runtimeMaterial;
        }

        private void OnDestroy()
        {
            if (_runtimeMaterial != null)
            {
                Destroy(_runtimeMaterial);
            }
        }

        private void OnValidate()
        {
            renderRadius = Mathf.Max(0, renderRadius);
            chunksBelowFocus =
                Mathf.Max(0, chunksBelowFocus);
            chunksAboveFocus =
                Mathf.Max(0, chunksAboveFocus);
        }

        private sealed class ResidentBlockSource :
            IVoxelBlockSource
        {
            private readonly VoxelWorldData _world;
            private readonly HashSet<ChunkCoord> _resident;

            public ResidentBlockSource(
                VoxelWorldData world,
                HashSet<ChunkCoord> resident)
            {
                _world = world;
                _resident = resident;
            }

            public bool TryGetBlock(
                int worldX,
                int worldY,
                int worldZ,
                out BlockId block)
            {
                ChunkCoord coord =
                    ChunkCoord.FromWorldVoxel(
                        worldX,
                        worldY,
                        worldZ);

                if (!_resident.Contains(coord))
                {
                    block = BlockId.Air;
                    return false;
                }

                return _world.TryGetBlock(
                    worldX,
                    worldY,
                    worldZ,
                    out block);
            }
        }
    }
}
