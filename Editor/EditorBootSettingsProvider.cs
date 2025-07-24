using UnityEditor;
using UnityEngine;
using WhiteArrow.Bootstraping;

namespace WhiteArrowEditor.Bootstraping
{
    public static class EditorBootSettingsProvider
    {
        public const string RESOURCES_FOLDER_PATH = "Assets/Resources/";
        public const string REGISTRY_ASSET_PATH = RESOURCES_FOLDER_PATH + BootstrapingSettings.FILE_NAME + ".asset";


        private static BootstrapingSettings _cached;



        public static BootstrapingSettings Load()
        {
            if (_cached != null)
                return _cached;

            _cached = AssetDatabase.LoadAssetAtPath<BootstrapingSettings>(REGISTRY_ASSET_PATH);

            if (_cached == null)
            {
                if (!AssetDatabase.IsValidFolder(RESOURCES_FOLDER_PATH))
                    Project.CreateFullFolderPath(RESOURCES_FOLDER_PATH);

                _cached = ScriptableObject.CreateInstance<BootstrapingSettings>();
                AssetDatabase.CreateAsset(_cached, REGISTRY_ASSET_PATH);

                Debug.Log($"{REGISTRY_ASSET_PATH} asset created.");
            }

            return _cached;
        }

        public static void Save()
        {
            EditorUtility.SetDirty(_cached);
            AssetDatabase.SaveAssets();
        }
    }
}