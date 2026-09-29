using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using VoxDetroit.Detroit;
using VoxDetroit.Player;
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

            Transform player =
                CreatePlayer();

            CreateSun();

            GameObject worldObject =
                new GameObject("Vox Detroit World");

            VoxelWorldStreamer streamer =
                worldObject.AddComponent<VoxelWorldStreamer>();

            DetroitImportBootstrap importer =
                worldObject.AddComponent<DetroitImportBootstrap>();

            worldObject.AddComponent<PrototypeRuntimeDiagnostics>();

            ConfigureStreamer(
                streamer,
                player);

            ConfigureImporter(
                importer,
                data);

            EditorSceneManager.SaveScene(
                scene,
                ScenePath);

            Selection.activeGameObject =
                player.gameObject;

            EditorGUIUtility.PingObject(
                player.gameObject);

            Debug.Log(
                "Vox Detroit Downtown prototype scene created at " +
                ScenePath +
                ". Enter Play Mode for street-level exploration. " +
                "Controls: WASD/mouse, Shift sprint, Space jump, " +
                "F2 free-fly, F3 snap to street, Esc releases mouse.");
        }

        private static Transform CreatePlayer()
        {
            var playerObject =
                new GameObject("Prototype Player");

            playerObject.transform.position =
                new Vector3(
                    0f,
                    25f,
                    -30f);

            CharacterController character =
                playerObject.AddComponent<CharacterController>();

            character.height = 1.8f;
            character.radius = 0.34f;
            character.center =
                new Vector3(
                    0f,
                    0.9f,
                    0f);

            var cameraObject =
                new GameObject("Main Camera");

            cameraObject.tag = "MainCamera";

            cameraObject.transform.SetParent(
                playerObject.transform,
                false);

            cameraObject.transform.localPosition =
                new Vector3(
                    0f,
                    1.62f,
                    0f);

            Camera camera =
                cameraObject.AddComponent<Camera>();

            camera.nearClipPlane = 0.05f;
            camera.farClipPlane = 1800f;
            camera.fieldOfView = 72f;

            PrototypeFirstPersonController controller =
                playerObject.AddComponent<PrototypeFirstPersonController>();

            var serialized =
                new SerializedObject(controller);

            serialized.FindProperty("playerCamera")
                .objectReferenceValue =
                camera;

            serialized.ApplyModifiedPropertiesWithoutUndo();

            return playerObject.transform;
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
            Transform player)
        {
            var serialized =
                new SerializedObject(streamer);

            serialized.FindProperty("focus")
                .objectReferenceValue =
                player;

            serialized.FindProperty("renderRadius")
                .intValue = 8;

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
