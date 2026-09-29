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
        [Tooltip(
            "Optional material used as the base for generated " +
            "prototype block materials.")]
        [SerializeField] private Material voxelMaterial;

        private readonly Dictionary<ChunkCoord, VoxelChunkView> _views =
            new Dictionary<ChunkCoord, VoxelChunkView>();

        private readonly HashSet<ChunkCoord> _residentCoords =
            new HashSet<ChunkCoord>();

        private readonly VoxelWorldData _world =
            new VoxelWorldData();

        private ResidentBlockSource _residentBlockSource;
        private Material[] _runtimeMaterials;
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
                transform.InverseTransformPoint(
                    focus.position);

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

            foreach (VoxelChunkView view
                     in _views.Values)
            {
                view.RebuildMesh();
            }
        }

        private HashSet<ChunkCoord> BuildTargetSet(
            ChunkCoord center)
        {
            var target =
                new HashSet<ChunkCoord>();

            if (!generatePrototypeChunks &&
                _world.ChunkCount > 0)
            {
                foreach (ChunkCoord coord
                         in _world.ChunkCoords)
                {
                    if (System.Math.Abs(
                            coord.X - center.X) <=
                            renderRadius &&
                        System.Math.Abs(
                            coord.Z - center.Z) <=
                            renderRadius)
                    {
                        target.Add(coord);
                    }
                }

                return target;
            }

            int minY =
                center.Y - chunksBelowFocus;

            int maxY =
                center.Y + chunksAboveFocus;

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
            var remove =
                new List<ChunkCoord>();

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
                VoxelChunkView view =
                    _views[coord];

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

            renderer.sharedMaterials =
                ResolveMaterials();

            view.Initialize(
                coord,
                data,
                _residentBlockSource);

            return view;
        }

        private Material[] ResolveMaterials()
        {
            if (_runtimeMaterials != null)
            {
                return _runtimeMaterials;
            }

            Shader shader =
                voxelMaterial != null
                    ? voxelMaterial.shader
                    : Shader.Find(
                        "Universal Render Pipeline/Lit");

            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            if (shader == null)
            {
                Debug.LogError(
                    "Vox Detroit could not find a default shader.");

                _runtimeMaterials =
                    new Material[
                        BlockCatalog.MaterialSlotCount];

                return _runtimeMaterials;
            }

            _runtimeMaterials =
                new Material[
                    BlockCatalog.MaterialSlotCount];

            for (int slot = 0;
                 slot < _runtimeMaterials.Length;
                 slot++)
            {
                Material material =
                    voxelMaterial != null
                        ? new Material(voxelMaterial)
                        : new Material(shader);

                BlockId block = (BlockId)slot;

                material.name =
                    $"Runtime {block} Material";

                material.color =
                    GetPrototypeColor(block);

                _runtimeMaterials[slot] =
                    material;
            }

            return _runtimeMaterials;
        }

        private static Color GetPrototypeColor(
            BlockId block)
        {
            switch (block)
            {
                case BlockId.Soil:
                    return new Color(
                        0.30f,
                        0.20f,
                        0.12f);

                case BlockId.Grass:
                    return new Color(
                        0.22f,
                        0.42f,
                        0.20f);

                case BlockId.Concrete:
                    return new Color(
                        0.58f,
                        0.59f,
                        0.60f);

                case BlockId.Asphalt:
                    return new Color(
                        0.12f,
                        0.13f,
                        0.14f);

                case BlockId.Brick:
                    return new Color(
                        0.50f,
                        0.22f,
                        0.16f);

                case BlockId.Glass:
                    return new Color(
                        0.35f,
                        0.58f,
                        0.68f);

                case BlockId.Wood:
                    return new Color(
                        0.45f,
                        0.30f,
                        0.16f);

                case BlockId.Water:
                    return new Color(
                        0.18f,
                        0.38f,
                        0.62f);

                case BlockId.RoadMarking:
                    return new Color(
                        0.90f,
                        0.86f,
                        0.60f);

                default:
                    return new Color(
                        0.45f,
                        0.45f,
                        0.45f);
            }
        }

        private void OnDestroy()
        {
            if (_runtimeMaterials == null)
            {
                return;
            }

            foreach (Material material
                     in _runtimeMaterials)
            {
                if (material != null)
                {
                    Destroy(material);
                }
            }

            _runtimeMaterials = null;
        }

        private void OnValidate()
        {
            renderRadius =
                Mathf.Max(0, renderRadius);

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
