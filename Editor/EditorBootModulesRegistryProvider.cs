using UnityEditor;
using UnityEngine;
using WhiteArrow.Bootstraping;

namespace WhiteArrowEditor.Bootstraping
{
    public static class EditorBootModulesRegistryProvider
    {
        public const string RESOURCES_FOLDER_PATH = "Assets/Resources/";
        public const string REGISTRY_ASSET_PATH = RESOURCES_FOLDER_PATH + GameBootModulesRegistry.FILE_NAME + ".asset";


        private static GameBootModulesRegistry _cached;



        public static GameBootModulesRegistry Load()
        {
            if (_cached != null)
                return _cached;

            _cached = AssetDatabase.LoadAssetAtPath<GameBootModulesRegistry>(REGISTRY_ASSET_PATH);

            if (_cached == null)
            {
                if (!AssetDatabase.IsValidFolder(RESOURCES_FOLDER_PATH))
                    Project.CreateFullFolderPath(RESOURCES_FOLDER_PATH);

                _cached = ScriptableObject.CreateInstance<GameBootModulesRegistry>();
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