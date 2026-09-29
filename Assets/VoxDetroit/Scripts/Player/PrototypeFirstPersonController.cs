using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using VoxDetroit.Core;
using VoxDetroit.Voxels;
using VoxDetroit.World;

namespace VoxDetroit.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PrototypeFirstPersonController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera playerCamera;
        [SerializeField] private VoxelWorldStreamer worldStreamer;

        [Header("Walk")]
        [SerializeField, Min(0.1f)] private float walkSpeed = 5.5f;
        [SerializeField, Min(0.1f)] private float sprintSpeed = 9f;
        [SerializeField, Min(0.1f)] private float jumpHeight = 1.15f;
        [SerializeField] private float gravity = -24f;

        [Header("Look")]
        [SerializeField, Min(0.01f)] private float mouseSensitivity = 0.085f;
        [SerializeField, Range(30f, 89f)] private float maxPitch = 85f;

        [Header("Debug Fly")]
        [SerializeField, Min(1f)] private float flySpeed = 18f;
        [SerializeField, Min(1f)] private float flySprintSpeed = 45f;

        [Header("Prototype Spawn")]
        [SerializeField] private bool autoSnapToGround = true;
        [SerializeField, Min(10f)] private float voxelStreetSearchRadius = 150f;
        [SerializeField, Min(1f)] private float spawnSearchRadius = 32f;
        [SerializeField, Min(10f)] private float spawnProbeHeight = 220f;

        private CharacterController _controller;
        private float _verticalVelocity;
        private float _pitch;
        private bool _flyMode;
        private bool _cursorLocked = true;
        private bool _initialSpawnPending;
        private string _spawnStatus = "Waiting";

        public bool FlyMode => _flyMode;
        public string SpawnStatus => _spawnStatus;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();

            if (playerCamera == null)
            {
                playerCamera = GetComponentInChildren<Camera>();
            }

            if (worldStreamer == null)
            {
                worldStreamer =
                    FindFirstObjectByType<VoxelWorldStreamer>();
            }

            ConfigureCharacterController();
        }

        private void Start()
        {
            SetCursorLocked(true);

            if (autoSnapToGround)
            {
                StartCoroutine(
                    ResolveInitialStreetSpawn());
            }
            else
            {
                _spawnStatus = "Auto spawn disabled";
            }
        }

        private void Update()
        {
            Keyboard keyboard = Keyboard.current;
            Mouse mouse = Mouse.current;

            if (keyboard == null)
            {
                return;
            }

            HandleModeKeys(keyboard);

            if (_cursorLocked && mouse != null)
            {
                HandleLook(mouse);
            }

            if (_initialSpawnPending)
            {
                return;
            }

            if (_flyMode)
            {
                MoveFly(keyboard);
            }
            else
            {
                MoveWalk(keyboard);
            }
        }

        private void HandleModeKeys(Keyboard keyboard)
        {
            if (keyboard.escapeKey.wasPressedThisFrame)
            {
                SetCursorLocked(false);
            }

            if (!_cursorLocked &&
                Mouse.current != null &&
                Mouse.current.leftButton.wasPressedThisFrame)
            {
                SetCursorLocked(true);
            }

            if (keyboard.f2Key.wasPressedThisFrame)
            {
                SetFlyMode(!_flyMode);
            }

            if (keyboard.f3Key.wasPressedThisFrame)
            {
                if (_flyMode)
                {
                    SetFlyMode(false);
                }

                SnapToNearbyStreetLevel();
            }
        }

        private void HandleLook(Mouse mouse)
        {
            Vector2 delta =
                mouse.delta.ReadValue() *
                mouseSensitivity;

            transform.Rotate(
                Vector3.up,
                delta.x,
                Space.World);

            _pitch =
                Mathf.Clamp(
                    _pitch - delta.y,
                    -maxPitch,
                    maxPitch);

            if (playerCamera != null)
            {
                playerCamera.transform.localRotation =
                    Quaternion.Euler(
                        _pitch,
                        0f,
                        0f);
            }
        }

        private void MoveWalk(Keyboard keyboard)
        {
            if (!_controller.enabled)
            {
                _controller.enabled = true;
            }

            Vector2 input =
                ReadMoveInput(keyboard);

            Vector3 wish =
                (transform.forward * input.y) +
                (transform.right * input.x);

            if (wish.sqrMagnitude > 1f)
            {
                wish.Normalize();
            }

            bool sprint =
                keyboard.leftShiftKey.isPressed ||
                keyboard.rightShiftKey.isPressed;

            float speed =
                sprint
                    ? sprintSpeed
                    : walkSpeed;

            bool grounded =
                _controller.isGrounded;

            if (grounded &&
                _verticalVelocity < 0f)
            {
                _verticalVelocity = -2f;
            }

            if (grounded &&
                keyboard.spaceKey.wasPressedThisFrame)
            {
                _verticalVelocity =
                    Mathf.Sqrt(
                        jumpHeight *
                        -2f *
                        gravity);
            }

            _verticalVelocity +=
                gravity * Time.deltaTime;

            Vector3 motion =
                (wish * speed) +
                (Vector3.up * _verticalVelocity);

            _controller.Move(
                motion * Time.deltaTime);
        }

        private void MoveFly(Keyboard keyboard)
        {
            if (_controller.enabled)
            {
                _controller.enabled = false;
            }

            Vector2 input =
                ReadMoveInput(keyboard);

            Transform view =
                playerCamera != null
                    ? playerCamera.transform
                    : transform;

            Vector3 motion =
                (view.forward * input.y) +
                (view.right * input.x);

            if (keyboard.spaceKey.isPressed)
            {
                motion += Vector3.up;
            }

            if (keyboard.leftCtrlKey.isPressed ||
                keyboard.rightCtrlKey.isPressed)
            {
                motion += Vector3.down;
            }

            if (motion.sqrMagnitude > 1f)
            {
                motion.Normalize();
            }

            bool sprint =
                keyboard.leftShiftKey.isPressed ||
                keyboard.rightShiftKey.isPressed;

            float speed =
                sprint
                    ? flySprintSpeed
                    : flySpeed;

            transform.position +=
                motion *
                speed *
                Time.deltaTime;
        }

        private static Vector2 ReadMoveInput(
            Keyboard keyboard)
        {
            float x = 0f;
            float y = 0f;

            if (keyboard.aKey.isPressed)
            {
                x -= 1f;
            }

            if (keyboard.dKey.isPressed)
            {
                x += 1f;
            }

            if (keyboard.sKey.isPressed)
            {
                y -= 1f;
            }

            if (keyboard.wKey.isPressed)
            {
                y += 1f;
            }

            return new Vector2(x, y);
        }

        private void SetFlyMode(bool enabled)
        {
            _flyMode = enabled;
            _verticalVelocity = 0f;

            if (_controller != null)
            {
                _controller.enabled = !enabled;
            }

            _spawnStatus =
                enabled
                    ? "Debug fly"
                    : "Walking";
        }

        private void SetCursorLocked(bool locked)
        {
            _cursorLocked = locked;

            Cursor.lockState =
                locked
                    ? CursorLockMode.Locked
                    : CursorLockMode.None;

            Cursor.visible = !locked;
        }

        private void ConfigureCharacterController()
        {
            _controller.height = 1.8f;
            _controller.radius = 0.34f;
            _controller.center =
                new Vector3(
                    0f,
                    0.9f,
                    0f);
            _controller.stepOffset = 0.35f;
            _controller.slopeLimit = 50f;
            _controller.skinWidth = 0.04f;
        }

        private IEnumerator ResolveInitialStreetSpawn()
        {
            _initialSpawnPending = true;
            _spawnStatus = "Waiting for Detroit data";

            bool controllerWasEnabled =
                _controller != null &&
                _controller.enabled;

            if (_controller != null)
            {
                _controller.enabled = false;
            }

            const int maxAttempts = 120;

            for (int attempt = 0;
                 attempt < maxAttempts;
                 attempt++)
            {
                yield return null;

                if (worldStreamer == null)
                {
                    worldStreamer =
                        FindFirstObjectByType<VoxelWorldStreamer>();
                }

                if (worldStreamer == null ||
                    worldStreamer.World.ChunkCount <= 0)
                {
                    continue;
                }

                _spawnStatus = "Finding nearest road";

                if (TryFindRoadVoxel(
                        out Vector3 roadPoint))
                {
                    transform.position = roadPoint;
                    Physics.SyncTransforms();

                    _initialSpawnPending = false;
                    _spawnStatus = "Street spawn";

                    if (_controller != null)
                    {
                        _controller.enabled =
                            controllerWasEnabled;
                    }

                    if (worldStreamer != null)
                    {
                        worldStreamer.SetFocus(transform);
                    }

                    Debug.Log(
                        $"Vox Detroit player placed on imported road voxel at " +
                        $"{transform.position}.");

                    yield break;
                }
            }

            _initialSpawnPending = false;

            if (TryFindNearbyStreetLevelPhysics(
                    out Vector3 physicsPoint))
            {
                transform.position =
                    physicsPoint +
                    (Vector3.up * 0.08f);

                Physics.SyncTransforms();
                _spawnStatus = "Physics fallback";

                if (_controller != null)
                {
                    _controller.enabled =
                        controllerWasEnabled;
                }

                Debug.Log(
                    $"Vox Detroit player used physics spawn fallback at " +
                    $"{transform.position}.");

                yield break;
            }

            SetFlyMode(true);
            _spawnStatus = "Spawn failed - fly mode";

            Debug.LogWarning(
                "Vox Detroit could not find a road voxel or walkable " +
                "collider. Free-fly mode was enabled automatically.");
        }

        [ContextMenu("Snap To Nearby Street Level")]
        public void SnapToNearbyStreetLevel()
        {
            bool controllerWasEnabled =
                _controller != null &&
                _controller.enabled;

            if (_controller != null)
            {
                _controller.enabled = false;
            }

            if (worldStreamer == null)
            {
                worldStreamer =
                    FindFirstObjectByType<VoxelWorldStreamer>();
            }

            if (TryFindRoadVoxel(
                    out Vector3 roadPoint))
            {
                transform.position = roadPoint;
                Physics.SyncTransforms();
                _spawnStatus = "Street snap";

                if (_controller != null)
                {
                    _controller.enabled =
                        controllerWasEnabled &&
                        !_flyMode;
                }

                if (worldStreamer != null)
                {
                    worldStreamer.SetFocus(transform);
                }

                Debug.Log(
                    $"Vox Detroit player snapped to imported road voxel at " +
                    $"{transform.position}.");

                return;
            }

            if (TryFindNearbyStreetLevelPhysics(
                    out Vector3 physicsPoint))
            {
                transform.position =
                    physicsPoint +
                    (Vector3.up * 0.08f);

                Physics.SyncTransforms();
                _spawnStatus = "Physics street snap";

                if (_controller != null)
                {
                    _controller.enabled =
                        controllerWasEnabled &&
                        !_flyMode;
                }

                return;
            }

            if (_controller != null)
            {
                _controller.enabled =
                    controllerWasEnabled &&
                    !_flyMode;
            }

            _spawnStatus = "No nearby street";

            Debug.LogWarning(
                "Vox Detroit player could not find a nearby imported " +
                "road voxel or walkable surface.");
        }

        private bool TryFindRoadVoxel(
            out Vector3 worldPoint)
        {
            worldPoint = transform.position;

            if (worldStreamer == null ||
                worldStreamer.World.ChunkCount <= 0)
            {
                return false;
            }

            float voxelSize =
                VoxDetroitConstants.VoxelSizeMeters;

            int centerX =
                Mathf.FloorToInt(
                    transform.position.x /
                    voxelSize);

            int centerZ =
                Mathf.FloorToInt(
                    transform.position.z /
                    voxelSize);

            int maxRadius =
                Mathf.CeilToInt(
                    voxelStreetSearchRadius /
                    voxelSize);

            if (FindNearestGroundBlock(
                    centerX,
                    centerZ,
                    maxRadius,
                    BlockId.Asphalt,
                    out int roadX,
                    out int roadZ))
            {
                worldPoint =
                    ToStandingPoint(
                        roadX,
                        roadZ,
                        0);

                return true;
            }

            BlockId[] fallbacks =
            {
                BlockId.Concrete,
                BlockId.Grass,
                BlockId.Soil
            };

            foreach (BlockId fallback in fallbacks)
            {
                if (FindNearestGroundBlock(
                        centerX,
                        centerZ,
                        maxRadius,
                        fallback,
                        out int fallbackX,
                        out int fallbackZ))
                {
                    worldPoint =
                        ToStandingPoint(
                            fallbackX,
                            fallbackZ,
                            0);

                    return true;
                }
            }

            return false;
        }

        private bool FindNearestGroundBlock(
            int centerX,
            int centerZ,
            int maxRadius,
            BlockId required,
            out int foundX,
            out int foundZ)
        {
            foundX = 0;
            foundZ = 0;

            for (int radius = 0;
                 radius <= maxRadius;
                 radius++)
            {
                int minX = centerX - radius;
                int maxX = centerX + radius;
                int minZ = centerZ - radius;
                int maxZ = centerZ + radius;

                for (int x = minX; x <= maxX; x++)
                {
                    if (IsValidGround(
                            x,
                            minZ,
                            required))
                    {
                        foundX = x;
                        foundZ = minZ;
                        return true;
                    }

                    if (radius > 0 &&
                        IsValidGround(
                            x,
                            maxZ,
                            required))
                    {
                        foundX = x;
                        foundZ = maxZ;
                        return true;
                    }
                }

                for (int z = minZ + 1;
                     z < maxZ;
                     z++)
                {
                    if (IsValidGround(
                            minX,
                            z,
                            required))
                    {
                        foundX = minX;
                        foundZ = z;
                        return true;
                    }

                    if (radius > 0 &&
                        IsValidGround(
                            maxX,
                            z,
                            required))
                    {
                        foundX = maxX;
                        foundZ = z;
                        return true;
                    }
                }
            }

            return false;
        }

        private bool IsValidGround(
            int x,
            int z,
            BlockId required)
        {
            VoxelWorldData world =
                worldStreamer.World;

            return
                world.GetBlockOrAir(
                    x,
                    0,
                    z) == required &&
                world.GetBlockOrAir(
                    x,
                    1,
                    z) == BlockId.Air &&
                world.GetBlockOrAir(
                    x,
                    2,
                    z) == BlockId.Air &&
                world.GetBlockOrAir(
                    x,
                    3,
                    z) == BlockId.Air;
        }

        private static Vector3 ToStandingPoint(
            int voxelX,
            int voxelZ,
            int voxelY)
        {
            float size =
                VoxDetroitConstants.VoxelSizeMeters;

            return new Vector3(
                (voxelX + 0.5f) * size,
                ((voxelY + 1f) * size) + 0.08f,
                (voxelZ + 0.5f) * size);
        }

        private bool TryFindNearbyStreetLevelPhysics(
            out Vector3 bestPoint)
        {
            Physics.SyncTransforms();

            Vector3 center =
                transform.position;

            float bestY =
                float.PositiveInfinity;

            bestPoint =
                center;

            bool found = false;

            const int samplesPerAxis = 9;

            for (int xi = 0;
                 xi < samplesPerAxis;
                 xi++)
            {
                float tx =
                    xi /
                    (float)(samplesPerAxis - 1);

                float offsetX =
                    Mathf.Lerp(
                        -spawnSearchRadius,
                        spawnSearchRadius,
                        tx);

                for (int zi = 0;
                     zi < samplesPerAxis;
                     zi++)
                {
                    float tz =
                        zi /
                        (float)(samplesPerAxis - 1);

                    float offsetZ =
                        Mathf.Lerp(
                            -spawnSearchRadius,
                            spawnSearchRadius,
                            tz);

                    Vector3 origin =
                        new Vector3(
                            center.x + offsetX,
                            spawnProbeHeight,
                            center.z + offsetZ);

                    if (!Physics.Raycast(
                            origin,
                            Vector3.down,
                            out RaycastHit hit,
                            spawnProbeHeight * 2f,
                            ~0,
                            QueryTriggerInteraction.Ignore))
                    {
                        continue;
                    }

                    if (hit.normal.y < 0.65f)
                    {
                        continue;
                    }

                    if (hit.point.y < bestY)
                    {
                        bestY = hit.point.y;
                        bestPoint = hit.point;
                        found = true;
                    }
                }
            }

            return found;
        }
    }
}
