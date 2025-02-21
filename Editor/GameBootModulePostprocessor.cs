using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace WhiteArrow.Bootstraping.Editor
{
    /// <summary>
    /// Updates the BootstrapModuleRegistry whenever scripts are recompiled or new assets are imported.
    /// </summary>
    public class GameBootModulePostprocessor : AssetPostprocessor
    {
        private static GameBootModuleRegistry _bootModuleRegistry;


        public const string RESOURCES_FOLDER_PATH = "Assets/Resources/";
        public const string REGISTRY_ASSET_PATH = RESOURCES_FOLDER_PATH + GameBootModuleRegistry.FILE_NAME + ".asset";



        private GameBootModulePostprocessor()
        {
            CompilationPipeline.assemblyCompilationFinished += OnScriptsCompiled;
        }

        private static void OnScriptsCompiled(string assemblyPath, CompilerMessage[] messages)
        {
            UpdateBootstrapModuleRegistry();
        }



        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
        {
            if (importedAssets.Any(asset => asset.EndsWith(".cs")))
            {
                UpdateBootstrapModuleRegistry();
            }
        }




        [MenuItem("Tools/WhiteArrow/Bootstraping/Enable", true)]
        private static bool ValidateEnableBootstraping()
        {
            LoadBootModuleRegistry();
            return !_bootModuleRegistry.BootstrapingIsEnabled;
        }

        [MenuItem("Tools/WhiteArrow/Bootstraping/Enable")]
        private static void EnableBootstraping()
        {
            LoadBootModuleRegistry();
            _bootModuleRegistry.BootstrapingIsEnabled = true;
            SaveBootModuleRegistry();
        }



        [MenuItem("Tools/WhiteArrow/Bootstraping/Disable", true)]
        private static bool ValidateDisableBootstraping()
        {
            LoadBootModuleRegistry();
            return _bootModuleRegistry.BootstrapingIsEnabled;
        }

        [MenuItem("Tools/WhiteArrow/Bootstraping/Disable")]
        private static void DisableBootstraping()
        {
            LoadBootModuleRegistry();
            _bootModuleRegistry.BootstrapingIsEnabled = false;
            SaveBootModuleRegistry();
        }



        /// <summary>
        /// Scans for GameBootstrapModule types and updates the registry.
        /// </summary>
        private static void UpdateBootstrapModuleRegistry()
        {
            // Find all valid module types.
            var moduleTypes = System.Reflection.Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(type => type.IsClass
                               && !type.IsAbstract
                               && type.IsSubclassOf(typeof(GameBootModule))
                               && type.GetCustomAttribute<GameBootModuleAttribute>() != null)
                .Select(type => type.FullName)
                .ToList();

            LoadBootModuleRegistry();

            // Update the registry only if there's a change in the module list.
            if (!moduleTypes.SequenceEqual(_bootModuleRegistry.ModuleTypeNames))
            {
                _bootModuleRegistry.ModuleTypeNames = moduleTypes;
                SaveBootModuleRegistry();
                Debug.Log($"{nameof(GameBootModuleRegistry)} updated: {moduleTypes.Count} modules found.");
            }
        }



        private static void LoadBootModuleRegistry()
        {
            if (_bootModuleRegistry != null)
                return;


            _bootModuleRegistry = AssetDatabase.LoadAssetAtPath<GameBootModuleRegistry>(REGISTRY_ASSET_PATH);

            if (_bootModuleRegistry == null)
            {
                if (!AssetDatabase.IsValidFolder(RESOURCES_FOLDER_PATH))
                    Project.CreateFullFolderPath(RESOURCES_FOLDER_PATH);

                _bootModuleRegistry = ScriptableObject.CreateInstance<GameBootModuleRegistry>();
                AssetDatabase.CreateAsset(_bootModuleRegistry, REGISTRY_ASSET_PATH);

                Debug.Log($"{REGISTRY_ASSET_PATH} asset created.");
            }
        }

        private static void SaveBootModuleRegistry()
        {
            EditorUtility.SetDirty(_bootModuleRegistry);
            AssetDatabase.SaveAssets();
        }
    }
}
