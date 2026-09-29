using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace VoxDetroit.Events
{
    public sealed class PrototypeCrowdVisual : MonoBehaviour
    {
        [SerializeField, Min(0)]
        private int expectedAttendance = 10000;

        [SerializeField, Range(25, 150)]
        private int performanceBudgetPercent = 100;

        [SerializeField, Min(100)]
        private int visualInstanceCap = 5000;

        [SerializeField]
        private Vector2 areaSize = new Vector2(90f, 60f);

        [SerializeField]
        private float baseHeight = 0f;

        [SerializeField]
        private int seed = 42701;

        [SerializeField]
        private Material crowdMaterial;

        private Mesh _mesh;
        private Material _runtimeMaterial;
        private Matrix4x4[] _matrices =
            Array.Empty<Matrix4x4>();

        public int DrawnVisualInstances =>
            _matrices?.Length ?? 0;

        public int RepresentedAttendance =>
            Math.Max(0, expectedAttendance);

        public int ApproximatePeoplePerVisualInstance =>
            DrawnVisualInstances <= 0
                ? 0
                : Mathf.Max(
                    1,
                    Mathf.CeilToInt(
                        RepresentedAttendance /
                        (float)DrawnVisualInstances));

        private void Awake()
        {
            Rebuild();
        }

        private void OnEnable()
        {
            if (_matrices == null ||
                _matrices.Length == 0)
            {
                Rebuild();
            }
        }

        private void Update()
        {
            DrawCrowd();
        }

        [ContextMenu("Rebuild Prototype Crowd")]
        public void Rebuild()
        {
            EnsureRenderResources();

            CrowdRepresentationPlan plan =
                CrowdRepresentationPlanner.Build(
                    expectedAttendance,
                    performanceBudgetPercent);

            int desired =
                Math.Min(
                    plan.visualCrowdCount,
                    visualInstanceCap);

            if (desired <= 0 &&
                expectedAttendance > 0)
            {
                desired =
                    Math.Min(
                        expectedAttendance,
                        Math.Min(
                            500,
                            visualInstanceCap));
            }

            _matrices =
                BuildMatrices(
                    desired,
                    seed,
                    areaSize,
                    baseHeight);
        }

        private void DrawCrowd()
        {
            if (_mesh == null ||
                ResolveMaterial() == null ||
                _matrices == null ||
                _matrices.Length == 0)
            {
                return;
            }

            const int batchSize = 1023;
            var batch =
                new Matrix4x4[batchSize];

            for (int offset = 0;
                 offset < _matrices.Length;
                 offset += batchSize)
            {
                int count =
                    Math.Min(
                        batchSize,
                        _matrices.Length - offset);

                Array.Copy(
                    _matrices,
                    offset,
                    batch,
                    0,
                    count);

                Graphics.DrawMeshInstanced(
                    _mesh,
                    0,
                    ResolveMaterial(),
                    batch,
                    count,
                    null,
                    ShadowCastingMode.Off,
                    false,
                    gameObject.layer);
            }
        }

        private void EnsureRenderResources()
        {
            if (_mesh == null)
            {
                _mesh = CreateCrossedBillboardMesh();
            }

            ResolveMaterial();
        }

        private Material ResolveMaterial()
        {
            if (crowdMaterial != null)
            {
                crowdMaterial.enableInstancing = true;
                return crowdMaterial;
            }

            if (_runtimeMaterial != null)
            {
                return _runtimeMaterial;
            }

            Shader shader =
                Shader.Find(
                    "Universal Render Pipeline/Unlit");

            if (shader == null)
            {
                shader = Shader.Find("Unlit/Color");
            }

            if (shader == null)
            {
                shader = Shader.Find("Standard");
            }

            if (shader == null)
            {
                return null;
            }

            _runtimeMaterial =
                new Material(shader)
                {
                    name =
                        "Prototype Crowd Material",
                    color =
                        new Color(
                            0.12f,
                            0.14f,
                            0.18f)
                };

            _runtimeMaterial.enableInstancing = true;
            return _runtimeMaterial;
        }

        private static Matrix4x4[] BuildMatrices(
            int count,
            int seed,
            Vector2 size,
            float baseHeight)
        {
            var matrices =
                new Matrix4x4[Math.Max(0, count)];

            var rng =
                new DeterministicRng(seed);

            for (int i = 0; i < matrices.Length; i++)
            {
                float x =
                    ((rng.NextFloat() - 0.5f) *
                     Mathf.Max(1f, size.x));

                float z =
                    ((rng.NextFloat() - 0.5f) *
                     Mathf.Max(1f, size.y));

                float scale =
                    0.8f +
                    (rng.NextFloat() * 0.35f);

                float yaw =
                    rng.NextFloat() * 360f;

                matrices[i] =
                    Matrix4x4.TRS(
                        new Vector3(
                            x,
                            baseHeight,
                            z),
                        Quaternion.Euler(
                            0f,
                            yaw,
                            0f),
                        new Vector3(
                            scale,
                            scale,
                            scale));
            }

            return matrices;
        }

        private static Mesh CreateCrossedBillboardMesh()
        {
            var mesh = new Mesh
            {
                name =
                    "Prototype Crowd Cross Billboard"
            };

            var vertices =
                new List<Vector3>
                {
                    new Vector3(-0.22f, 0f, 0f),
                    new Vector3(-0.22f, 1.75f, 0f),
                    new Vector3(0.22f, 1.75f, 0f),
                    new Vector3(0.22f, 0f, 0f),

                    new Vector3(0f, 0f, -0.22f),
                    new Vector3(0f, 1.75f, -0.22f),
                    new Vector3(0f, 1.75f, 0.22f),
                    new Vector3(0f, 0f, 0.22f)
                };

            int[] triangles =
            {
                0, 1, 2,
                0, 2, 3,
                2, 1, 0,
                3, 2, 0,

                4, 5, 6,
                4, 6, 7,
                6, 5, 4,
                7, 6, 4
            };

            mesh.SetVertices(vertices);
            mesh.SetTriangles(triangles, 0);
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }

        private void OnDestroy()
        {
            if (_mesh != null)
            {
                Destroy(_mesh);
            }

            if (_runtimeMaterial != null)
            {
                Destroy(_runtimeMaterial);
            }
        }

        private sealed class DeterministicRng
        {
            private uint _state;

            public DeterministicRng(int seed)
            {
                _state =
                    seed == 0
                        ? 2463534242u
                        : unchecked((uint)seed);
            }

            public float NextFloat()
            {
                _state ^= _state << 13;
                _state ^= _state >> 17;
                _state ^= _state << 5;

                return
                    (_state & 0x00FFFFFF) /
                    16777216f;
            }
        }
    }
}
