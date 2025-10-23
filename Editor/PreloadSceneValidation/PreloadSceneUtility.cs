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
            if (EditorBuildSettings.scenes.Length <= 0)
                return false;

            var firstScene = EditorBuildSettings.scenes[0];
            var name = SceneLoader.INTERMEDIATE_SCENE_NAME;
            return firstScene.path.Contains($"/{name}.unity");
        }



        public static void ShowFixDialog()
        {
            const string title = "Bootstraping Issue";
            const string message = "The preload scene is either missing or not placed at index 0 in Build Settings.\n\nIt will now be automatically fixed.";
            EditorUtility.DisplayDialog(title, message, "OK");
            FixIssue();
        }



        [MenuItem("Tools/WhiteArrow/Bootstraping/Fix Preload Scene Issue", true)]
        private static bool ValidateFixPreloadSceneIssue()
        {
            return !PreloadSceneUtility.IsValid();
        }

        [MenuItem("Tools/WhiteArrow/Bootstraping/Fix Preload Scene Issue")]
        public static void FixIssue()
        {
            var sceneExists = EditorBuildSettings.scenes.Any(s => s.path == PRELOAD_SCENE_OUTPUT_PATH);
            if (!sceneExists)
                GeneratePreloadScene();

            AddToBuildSettings(PRELOAD_SCENE_OUTPUT_PATH);
        }



        private static void GeneratePreloadScene()
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

            if (scenes.Count > 0 && scenes[0].path == scenePath)
                return;

            scenes.RemoveAll(s => s.path == scenePath);
            scenes.Insert(0, new EditorBuildSettingsScene(scenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();

            Debug.Log("Preload scene added to Build Settings at index 0.");
        }
    }
}