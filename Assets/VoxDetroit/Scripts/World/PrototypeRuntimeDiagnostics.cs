using UnityEngine;

namespace VoxDetroit.World
{
    [RequireComponent(typeof(VoxelWorldStreamer))]
    public sealed class PrototypeRuntimeDiagnostics : MonoBehaviour
    {
        private VoxelWorldStreamer _streamer;

        private void Awake()
        {
            _streamer =
                GetComponent<VoxelWorldStreamer>();
        }

        private void OnGUI()
        {
            if (_streamer == null)
            {
                return;
            }

            GUI.Box(
                new Rect(12f, 12f, 310f, 92f),
                "Vox Detroit Prototype");

            GUI.Label(
                new Rect(24f, 38f, 280f, 22f),
                $"World chunks: {_streamer.World.ChunkCount}");

            GUI.Label(
                new Rect(24f, 59f, 280f, 22f),
                $"Rendered chunks: {_streamer.ResidentViewCount}");

            GUI.Label(
                new Rect(24f, 80f, 280f, 22f),
                _streamer.World.ChunkCount > 0
                    ? "Detroit data imported"
                    : "Waiting for Detroit import...");
        }
    }
}
