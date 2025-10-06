using System;
using UnityEditor;
using UnityEngine;
using WhiteArrow.Bootstraping;

namespace WhiteArrowEditor.Bootstraping
{
    public static class EditorBootSettingsProvider
    {
        private static BootSettings s_settings;



        public const string RESOURCES_FOLDER_PATH = "Assets/Resources/";
        public const string SETTINGS_ASSET_PATH = RESOURCES_FOLDER_PATH + BootSettings.FILE_NAME + ".asset";



        public static BootSettings Settings
        {
            get
            {
                if (s_settings == null)
                    Load();

                return s_settings;
            }
        }



        private static void Load()
        {
            if (s_settings != null)
                return;

            s_settings = AssetDatabase.LoadAssetAtPath<BootSettings>(SETTINGS_ASSET_PATH);

            if (s_settings == null)
            {
                if (!AssetDatabase.IsValidFolder(RESOURCES_FOLDER_PATH))
                    CreateFullFolderPath(RESOURCES_FOLDER_PATH);

                s_settings = ScriptableObject.CreateInstance<BootSettings>();
                AssetDatabase.CreateAsset(s_settings, SETTINGS_ASSET_PATH);

                Debug.Log($"{SETTINGS_ASSET_PATH} asset created in {RESOURCES_FOLDER_PATH}.");
                EditorUtility.DisplayDialog("Bootstraping", "Bootstraping settings created.", "OK");
            }
        }

        public static void CreateFullFolderPath(string path)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                var partsOfPath = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
                var accumulatedPath = "Assets";

                foreach (var part in partsOfPath)
                {
                    if (part == "Assets")
                        continue;

                    AssetDatabase.CreateFolder(accumulatedPath, part);
                    accumulatedPath += "/" + part;
                }
            }
        }

        public static void Save()
        {
            EditorUtility.SetDirty(s_settings);
            AssetDatabase.SaveAssets();
        }
    }
}