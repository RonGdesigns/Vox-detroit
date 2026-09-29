using UnityEngine;

namespace VoxDetroit.Events
{
    public sealed class PrototypeEventOverlay : MonoBehaviour
    {
        [SerializeField]
        private bool sportsGame;

        [SerializeField]
        private CrowdTier crowdTier =
            CrowdTier.Packed;

        [SerializeField, Min(0)]
        private int expectedAttendance = 30000;

        [SerializeField, Min(0)]
        private int stageCount = 2;

        [SerializeField]
        private Vector2 footprint =
            new Vector2(90f, 65f);

        private const string GeneratedRootName =
            "__PrototypeEventOverlay";

        private void Start()
        {
            Rebuild();
        }

        [ContextMenu("Rebuild Prototype Event Overlay")]
        public void Rebuild()
        {
            ClearGenerated();

            EventOverlayPlan plan =
                sportsGame
                    ? EventOverlayPlanner.ForSportsGame(
                        expectedAttendance)
                    : EventOverlayPlanner.ForFestival(
                        crowdTier,
                        stageCount);

            Transform root =
                new GameObject(
                    GeneratedRootName)
                .transform;

            root.SetParent(
                transform,
                false);

            BuildStages(root, plan);
            BuildVendors(root, plan);
            BuildCheckpoints(root, plan);
            BuildBarrierPerimeter(root, plan);

            if (plan.tailgateZone)
            {
                CreateBox(
                    root,
                    "Tailgate Zone",
                    new Vector3(
                        -footprint.x * 0.32f,
                        0.08f,
                        -footprint.y * 0.32f),
                    new Vector3(
                        footprint.x * 0.28f,
                        0.15f,
                        footprint.y * 0.24f),
                    new Color(
                        0.32f,
                        0.28f,
                        0.20f));
            }
        }

        private void BuildStages(
            Transform root,
            EventOverlayPlan plan)
        {
            if (plan.stageCount <= 0)
            {
                return;
            }

            float spacing =
                footprint.x /
                (plan.stageCount + 1f);

            for (int i = 0;
                 i < plan.stageCount;
                 i++)
            {
                float x =
                    (-footprint.x * 0.5f) +
                    spacing * (i + 1);

                CreateBox(
                    root,
                    $"Stage {i + 1}",
                    new Vector3(
                        x,
                        1.5f,
                        footprint.y * 0.38f),
                    new Vector3(
                        14f,
                        3f,
                        8f),
                    new Color(
                        0.16f,
                        0.17f,
                        0.20f));
            }
        }

        private void BuildVendors(
            Transform root,
            EventOverlayPlan plan)
        {
            int count =
                Mathf.Min(
                    plan.vendorBooths,
                    50);

            for (int i = 0; i < count; i++)
            {
                int row = i / 10;
                int col = i % 10;

                float x =
                    -footprint.x * 0.42f +
                    col * 6.5f;

                float z =
                    -footprint.y * 0.38f +
                    row * 6f;

                CreateBox(
                    root,
                    $"Vendor {i + 1}",
                    new Vector3(
                        x,
                        1.1f,
                        z),
                    new Vector3(
                        4f,
                        2.2f,
                        3f),
                    new Color(
                        0.52f,
                        0.35f,
                        0.18f));
            }
        }

        private void BuildCheckpoints(
            Transform root,
            EventOverlayPlan plan)
        {
            int count =
                Mathf.Min(
                    plan.securityCheckpoints,
                    20);

            for (int i = 0; i < count; i++)
            {
                float t =
                    count <= 1
                        ? 0.5f
                        : i / (float)(count - 1);

                float x =
                    Mathf.Lerp(
                        -footprint.x * 0.42f,
                        footprint.x * 0.42f,
                        t);

                CreateBox(
                    root,
                    $"Checkpoint {i + 1}",
                    new Vector3(
                        x,
                        1.25f,
                        -footprint.y * 0.48f),
                    new Vector3(
                        2.2f,
                        2.5f,
                        1.1f),
                    new Color(
                        0.24f,
                        0.27f,
                        0.30f));
            }
        }

        private void BuildBarrierPerimeter(
            Transform root,
            EventOverlayPlan plan)
        {
            int count =
                Mathf.Clamp(
                    plan.temporaryBarrierSegments,
                    8,
                    80);

            for (int i = 0; i < count; i++)
            {
                float t =
                    i / (float)count;

                Vector3 position;
                Vector3 scale;

                if (t < 0.25f)
                {
                    float p = t / 0.25f;
                    position =
                        new Vector3(
                            Mathf.Lerp(
                                -footprint.x * 0.5f,
                                footprint.x * 0.5f,
                                p),
                            0.45f,
                            -footprint.y * 0.5f);
                    scale =
                        new Vector3(
                            footprint.x / count * 4.2f,
                            0.9f,
                            0.18f);
                }
                else if (t < 0.5f)
                {
                    float p =
                        (t - 0.25f) / 0.25f;
                    position =
                        new Vector3(
                            footprint.x * 0.5f,
                            0.45f,
                            Mathf.Lerp(
                                -footprint.y * 0.5f,
                                footprint.y * 0.5f,
                                p));
                    scale =
                        new Vector3(
                            0.18f,
                            0.9f,
                            footprint.y / count * 4.2f);
                }
                else if (t < 0.75f)
                {
                    float p =
                        (t - 0.5f) / 0.25f;
                    position =
                        new Vector3(
                            Mathf.Lerp(
                                footprint.x * 0.5f,
                                -footprint.x * 0.5f,
                                p),
                            0.45f,
                            footprint.y * 0.5f);
                    scale =
                        new Vector3(
                            footprint.x / count * 4.2f,
                            0.9f,
                            0.18f);
                }
                else
                {
                    float p =
                        (t - 0.75f) / 0.25f;
                    position =
                        new Vector3(
                            -footprint.x * 0.5f,
                            0.45f,
                            Mathf.Lerp(
                                footprint.y * 0.5f,
                                -footprint.y * 0.5f,
                                p));
                    scale =
                        new Vector3(
                            0.18f,
                            0.9f,
                            footprint.y / count * 4.2f);
                }

                CreateBox(
                    root,
                    $"Barrier {i + 1}",
                    position,
                    scale,
                    new Color(
                        0.65f,
                        0.65f,
                        0.67f));
            }
        }

        private static GameObject CreateBox(
            Transform parent,
            string name,
            Vector3 localPosition,
            Vector3 localScale,
            Color color)
        {
            GameObject box =
                GameObject.CreatePrimitive(
                    PrimitiveType.Cube);

            box.name = name;

            box.transform.SetParent(
                parent,
                false);

            box.transform.localPosition =
                localPosition;

            box.transform.localScale =
                localScale;

            Collider collider =
                box.GetComponent<Collider>();

            if (collider != null)
            {
                Destroy(collider);
            }

            Renderer renderer =
                box.GetComponent<Renderer>();

            if (renderer != null)
            {
                Shader shader =
                    Shader.Find(
                        "Universal Render Pipeline/Lit");

                if (shader == null)
                {
                    shader =
                        Shader.Find("Standard");
                }

                if (shader != null)
                {
                    Material material =
                        new Material(shader);

                    material.color = color;
                    renderer.sharedMaterial =
                        material;
                }
            }

            return box;
        }

        private void ClearGenerated()
        {
            Transform existing =
                transform.Find(
                    GeneratedRootName);

            if (existing == null)
            {
                return;
            }

            if (Application.isPlaying)
            {
                Destroy(existing.gameObject);
            }
            else
            {
                DestroyImmediate(
                    existing.gameObject);
            }
        }
    }
}
