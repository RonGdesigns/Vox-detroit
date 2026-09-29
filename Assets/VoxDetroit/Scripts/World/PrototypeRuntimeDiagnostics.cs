using UnityEngine;

namespace VoxDetroit.World
{
    [RequireComponent(typeof(VoxelWorldStreamer))]
    public sealed class PrototypeRuntimeDiagnostics : MonoBehaviour
    {
        private VoxelWorldStreamer _streamer;
        private float _smoothedFps;

        private void Awake()
        {
            _streamer =
                GetComponent<VoxelWorldStreamer>();
        }

        private void Update()
        {
            float instantaneous =
                1f / Mathf.Max(
                    0.0001f,
                    Time.unscaledDeltaTime);

            _smoothedFps =
                Mathf.Lerp(
                    _smoothedFps <= 0f
                        ? instantaneous
                        : _smoothedFps,
                    instantaneous,
                    0.08f);
        }

        private void OnGUI()
        {
            if (_streamer == null)
            {
                return;
            }

            GUI.Box(
                new Rect(12f, 12f, 360f, 148f),
                "Vox Detroit Prototype");

            GUI.Label(
                new Rect(24f, 38f, 330f, 22f),
                $"World chunks: {_streamer.World.ChunkCount}");

            GUI.Label(
                new Rect(24f, 59f, 330f, 22f),
                $"Rendered chunks: {_streamer.ResidentViewCount}");

            GUI.Label(
                new Rect(24f, 80f, 330f, 22f),
                $"Approx FPS: {_smoothedFps:F0}");

            GUI.Label(
                new Rect(24f, 101f, 330f, 22f),
                _streamer.World.ChunkCount > 0
                    ? "Detroit data imported"
                    : "Waiting for Detroit import...");

            GUI.Label(
                new Rect(24f, 122f, 330f, 32f),
                "WASD + mouse | Shift sprint | Space jump | " +
                "F2 fly | F3 street | Esc cursor");
        }
    }
}
