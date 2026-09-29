using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using VoxDetroit.Detroit;
using VoxDetroit.World;

namespace VoxDetroit.Editor
{
    public static class PrototypeSceneSetup
    {
        private const string DataPath =
            "Assets/VoxDetroit/Data/downtown-core-prototype.json";

        private const string ScenesFolder =
            "Assets/VoxDetroit/Scenes";

        private const string ScenePath =
            ScenesFolder + "/DowntownPrototype.unity";

        [MenuItem("Vox Detroit/Setup Downtown Prototype Scene")]
        public static void SetupDowntownPrototype()
        {
            TextAsset data =
                AssetDatabase.LoadAssetAtPath<TextAsset>(
                    DataPath);

            if (data == null)
            {
                EditorUtility.DisplayDialog(
                    "Vox Detroit",
                    "The Downtown prototype JSON was not found at:\n" +
                    DataPath,
                    "OK");

                return;
            }

            EnsureScenesFolder();

            Scene scene =
                EditorSceneManager.NewScene(
                    NewSceneSetup.EmptyScene,
                    NewSceneMode.Single);

            Camera camera = CreateCamera();
            CreateSun();

            GameObject worldObject =
                new GameObject("Vox Detroit World");

            VoxelWorldStreamer streamer =
                worldObject.AddComponent<VoxelWorldStreamer>();

            DetroitImportBootstrap importer =
                worldObject.AddComponent<DetroitImportBootstrap>();

            ConfigureStreamer(
                streamer,
                camera.transform);

            ConfigureImporter(
                importer,
                data);

            EditorSceneManager.SaveScene(
                scene,
                ScenePath);

            Selection.activeGameObject =
                worldObject;

            EditorGUIUtility.PingObject(
                worldObject);

            Debug.Log(
                "Vox Detroit Downtown prototype scene created at " +
                ScenePath +
                ". Enter Play Mode to import and render the city slice.");
        }

        private static Camera CreateCamera()
        {
            var cameraObject =
                new GameObject("Main Camera");

            cameraObject.tag = "MainCamera";

            Camera camera =
                cameraObject.AddComponent<Camera>();

            camera.nearClipPlane = 0.1f;
            camera.farClipPlane = 2000f;

            camera.transform.position =
                new Vector3(
                    0f,
                    110f,
                    -130f);

            camera.transform.LookAt(
                new Vector3(
                    0f,
                    25f,
                    0f));

            return camera;
        }

        private static void CreateSun()
        {
            var lightObject =
                new GameObject("Directional Light");

            Light light =
                lightObject.AddComponent<Light>();

            light.type =
                LightType.Directional;

            light.intensity = 1.1f;

            lightObject.transform.rotation =
                Quaternion.Euler(
                    50f,
                    -35f,
                    0f);
        }

        private static void ConfigureStreamer(
            VoxelWorldStreamer streamer,
            Transform cameraTransform)
        {
            var serialized =
                new SerializedObject(streamer);

            serialized.FindProperty("focus")
                .objectReferenceValue =
                cameraTransform;

            serialized.FindProperty("renderRadius")
                .intValue = 6;

            serialized.FindProperty(
                    "generatePrototypeChunks")
                .boolValue = false;

            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void ConfigureImporter(
            DetroitImportBootstrap importer,
            TextAsset data)
        {
            var serialized =
                new SerializedObject(importer);

            serialized.FindProperty("importJson")
                .objectReferenceValue = data;

            serialized.FindProperty("importOnAwake")
                .boolValue = true;

            serialized.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void EnsureScenesFolder()
        {
            if (AssetDatabase.IsValidFolder(
                    ScenesFolder))
            {
                return;
            }

            if (!AssetDatabase.IsValidFolder(
                    "Assets/VoxDetroit"))
            {
                Directory.CreateDirectory(
                    "Assets/VoxDetroit");

                AssetDatabase.Refresh();
            }

            AssetDatabase.CreateFolder(
                "Assets/VoxDetroit",
                "Scenes");
        }
    }
}
