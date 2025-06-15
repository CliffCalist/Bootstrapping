using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using WhiteArrow.Bootstraping;

namespace WhiteArrowEditor.Bootstraping
{
    /// <summary>
    /// Updates the BootstrapModuleRegistry whenever scripts are recompiled or new assets are imported.
    /// </summary>
    public class GameBootModulePostprocessor : AssetPostprocessor
    {
        private static GameBootModulesRegistry _bootModuleRegistry;


        public const string RESOURCES_FOLDER_PATH = "Assets/Resources/";
        public const string REGISTRY_ASSET_PATH = RESOURCES_FOLDER_PATH + GameBootModulesRegistry.FILE_NAME + ".asset";



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
            var moduleTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypesSafe())
                .Where(type => type.IsClass
                    && !type.IsAbstract
                    && type.IsSubclassOf(typeof(IGameBootModule)))
                .Select(type => type.AssemblyQualifiedName)
                .ToList();

            LoadBootModuleRegistry();

            // Update the registry only if there's a change in the module list.
            if (!moduleTypes.SequenceEqual(_bootModuleRegistry.ModuleTypeNames))
            {
                _bootModuleRegistry.ModuleTypeNames = moduleTypes;
                SaveBootModuleRegistry();
                Debug.Log($"{nameof(GameBootModulesRegistry)} updated: {moduleTypes.Count} modules found.");
            }
        }



        private static void LoadBootModuleRegistry()
        {
            if (_bootModuleRegistry != null)
                return;


            _bootModuleRegistry = AssetDatabase.LoadAssetAtPath<GameBootModulesRegistry>(REGISTRY_ASSET_PATH);

            if (_bootModuleRegistry == null)
            {
                if (!AssetDatabase.IsValidFolder(RESOURCES_FOLDER_PATH))
                    Project.CreateFullFolderPath(RESOURCES_FOLDER_PATH);

                _bootModuleRegistry = ScriptableObject.CreateInstance<GameBootModulesRegistry>();
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
