using UnityEngine;
using VoxDetroit.Player;

namespace VoxDetroit.World
{
    [RequireComponent(typeof(VoxelWorldStreamer))]
    public sealed class PrototypeRuntimeDiagnostics : MonoBehaviour
    {
        private VoxelWorldStreamer _streamer;
        private PrototypeFirstPersonController _player;
        private float _smoothedFps;

        private void Awake()
        {
            _streamer =
                GetComponent<VoxelWorldStreamer>();

            _player =
                FindFirstObjectByType<
                    PrototypeFirstPersonController>();
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

            if (_player == null)
            {
                _player =
                    FindFirstObjectByType<
                        PrototypeFirstPersonController>();
            }
        }

        private void OnGUI()
        {
            if (_streamer == null)
            {
                return;
            }

            GUI.Box(
                new Rect(12f, 12f, 430f, 190f),
                "Vox Detroit Prototype");

            GUI.Label(
                new Rect(24f, 38f, 400f, 22f),
                $"World chunks: {_streamer.World.ChunkCount}");

            GUI.Label(
                new Rect(24f, 59f, 400f, 22f),
                $"Rendered chunks: {_streamer.ResidentViewCount}");

            GUI.Label(
                new Rect(24f, 80f, 400f, 22f),
                $"Approx FPS: {_smoothedFps:F0}");

            GUI.Label(
                new Rect(24f, 101f, 400f, 22f),
                _streamer.World.ChunkCount > 0
                    ? "Detroit data imported"
                    : "Waiting for Detroit import...");

            if (_player != null)
            {
                Vector3 position =
                    _player.transform.position;

                GUI.Label(
                    new Rect(24f, 122f, 400f, 22f),
                    $"Player: X {position.x:F1}  Y {position.y:F1}  Z {position.z:F1}");

                GUI.Label(
                    new Rect(24f, 143f, 400f, 22f),
                    $"Mode: {(_player.FlyMode ? "Fly" : "Walk")} | Spawn: {_player.SpawnStatus}");
            }

            GUI.Label(
                new Rect(24f, 164f, 400f, 26f),
                "WASD + mouse | Shift sprint | Space jump | " +
                "F2 fly | F3 street | Esc cursor");
        }
    }
}
