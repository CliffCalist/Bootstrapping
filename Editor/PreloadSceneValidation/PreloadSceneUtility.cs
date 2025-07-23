using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using WhiteArrow.Bootstraping;

namespace WhiteArrowEditor.Bootstraping
{
    public static class PreloadSceneUtility
    {
        private const string PRELOAD_TEMPLATE_NAME = "PreloadSceneTemplate";
        private const string PRELOAD_SCENE_OUTPUT_PATH = "Assets/Preload.unity";



        public static bool IsValid()
        {
            var name = SceneLoader.INTERMEDIATE_SCENE_NAME;
            return EditorBuildSettings.scenes.Any(scene => scene.path.Contains($"/{name}.unity"));
        }



        public static void ShowMissingPreloadSceneDialog()
        {
            const string title = "Missing Preload Scene";
            const string message = "The preload scene is missing from Build Settings.\n\nIt will now be automatically created and added.";
            EditorUtility.DisplayDialog(title, message, "OK");
            GeneratePreloadScene();
        }


        public static void GeneratePreloadScene()
        {
            var templatePath = FindTemplatePath();
            if (string.IsNullOrEmpty(templatePath))
            {
                Debug.LogError($"Could not find scene asset named '{PRELOAD_TEMPLATE_NAME}'. Make sure it exists somewhere in the project.");
                return;
            }

            if (!AssetDatabase.CopyAsset(templatePath, PRELOAD_SCENE_OUTPUT_PATH))
            {
                Debug.LogError("Failed to copy preload scene template.");
                return;
            }

            Debug.Log($"Preload scene created at {PRELOAD_TEMPLATE_NAME}");
            AddToBuildSettings(PRELOAD_SCENE_OUTPUT_PATH);
            AssetDatabase.Refresh();
        }

        private static string FindTemplatePath()
        {
            var guids = AssetDatabase.FindAssets($"{PRELOAD_TEMPLATE_NAME} t:Scene");

            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (Path.GetFileNameWithoutExtension(path) == PRELOAD_TEMPLATE_NAME)
                    return path;
            }

            return null;
        }

        private static void AddToBuildSettings(string scenePath)
        {
            var scenes = EditorBuildSettings.scenes.ToList();

            if (scenes.Any(s => s.path == scenePath))
                return;

            scenes.Add(new EditorBuildSettingsScene(scenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();

            Debug.Log("Preload scene added to Build Settings.");
        }
    }
}