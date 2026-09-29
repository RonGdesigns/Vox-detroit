using UnityEngine;
using VoxDetroit.World;

namespace VoxDetroit.Detroit
{
    [RequireComponent(typeof(VoxelWorldStreamer))]
    public sealed class DetroitImportBootstrap : MonoBehaviour
    {
        [SerializeField] private TextAsset importJson;
        [SerializeField] private bool importOnAwake = true;

        private VoxelWorldStreamer _streamer;

        private void Awake()
        {
            _streamer = GetComponent<VoxelWorldStreamer>();

            if (importOnAwake)
            {
                Import();
            }
        }

        [ContextMenu("Import Detroit Data")]
        public void Import()
        {
            if (_streamer == null)
            {
                _streamer =
                    GetComponent<VoxelWorldStreamer>();
            }

            if (importJson == null)
            {
                Debug.LogWarning(
                    "No Detroit import JSON has been assigned.");
                return;
            }

            DetroitImportDocument document =
                JsonUtility.FromJson<DetroitImportDocument>(
                    importJson.text);

            if (document == null)
            {
                Debug.LogError(
                    "Detroit import JSON could not be parsed.");
                return;
            }

            _streamer.GeneratePrototypeChunks = false;
            _streamer.World.Clear();

            DetroitFeatureRasterizer.Rasterize(
                document,
                _streamer.World);

            _streamer.ForceRefresh();

            Debug.Log(
                $"Imported {document.areaName}: " +
                $"{_streamer.World.ChunkCount} chunks.");
        }
    }
}
