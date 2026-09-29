using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace VoxDetroit.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PrototypeFirstPersonController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Camera playerCamera;

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
        [SerializeField, Min(1f)] private float spawnSearchRadius = 32f;
        [SerializeField, Min(10f)] private float spawnProbeHeight = 220f;

        private CharacterController _controller;
        private float _verticalVelocity;
        private float _pitch;
        private bool _flyMode;
        private bool _cursorLocked = true;
        private bool _initialSpawnPending;

        public bool FlyMode => _flyMode;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();

            if (playerCamera == null)
            {
                playerCamera = GetComponentInChildren<Camera>();
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

            bool controllerWasEnabled =
                _controller != null &&
                _controller.enabled;

            if (_controller != null)
            {
                _controller.enabled = false;
            }

            const int maxAttempts = 60;

            for (int attempt = 0;
                 attempt < maxAttempts;
                 attempt++)
            {
                yield return null;

                if ((attempt % 3) == 0)
                {
                    yield return new WaitForFixedUpdate();
                }

                Physics.SyncTransforms();

                if (TryFindNearbyStreetLevel(
                        out Vector3 point))
                {
                    transform.position =
                        point +
                        (Vector3.up * 0.08f);

                    _initialSpawnPending = false;

                    if (_controller != null)
                    {
                        _controller.enabled =
                            controllerWasEnabled;
                    }

                    Debug.Log(
                        $"Vox Detroit player spawned at street level: " +
                        $"{transform.position}.");

                    yield break;
                }
            }

            _initialSpawnPending = false;
            SetFlyMode(true);

            Debug.LogWarning(
                "Vox Detroit could not resolve an initial street spawn " +
                "after waiting for voxel colliders. Free-fly mode was " +
                "enabled automatically; press F3 to retry.");

            if (_controller != null &&
                !_flyMode)
            {
                _controller.enabled =
                    controllerWasEnabled;
            }
        }

        [ContextMenu("Snap To Nearby Street Level")]
        public void SnapToNearbyStreetLevel()
        {
            if (TryFindNearbyStreetLevel(
                    out Vector3 point))
            {
                bool controllerWasEnabled =
                    _controller != null &&
                    _controller.enabled;

                if (_controller != null)
                {
                    _controller.enabled = false;
                }

                transform.position =
                    point +
                    (Vector3.up * 0.08f);

                Physics.SyncTransforms();

                if (_controller != null)
                {
                    _controller.enabled =
                        controllerWasEnabled &&
                        !_flyMode;
                }

                Debug.Log(
                    $"Vox Detroit player snapped to street level: " +
                    $"{transform.position}.");
            }
            else
            {
                Debug.LogWarning(
                    "Vox Detroit player could not find a nearby " +
                    "walkable surface. Press F2 for free-fly mode.");
            }
        }

        private bool TryFindNearbyStreetLevel(
            out Vector3 bestPoint)
        {
            bool controllerWasEnabled =
                _controller != null &&
                _controller.enabled;

            if (_controller != null)
            {
                _controller.enabled = false;
            }

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
                    xi / (float)(samplesPerAxis - 1);

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
                        zi / (float)(samplesPerAxis - 1);

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

            if (_controller != null)
            {
                _controller.enabled =
                    controllerWasEnabled &&
                    !_flyMode;
            }

            return found;
        }
    }
}
