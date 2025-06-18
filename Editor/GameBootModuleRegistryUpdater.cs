using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using WhiteArrow.Bootstraping;

namespace WhiteArrowEditor.Bootstraping
{
    /// <summary>
    /// Updates the BootstrapModuleRegistry whenever scripts are recompiled or new assets are imported.
    /// </summary>
    public class GameBootModuleRegistryUpdater
    {
        private static GameBootModulesRegistry _bootModuleRegistry => EditorBootModulesRegistryProvider.Load();



        private GameBootModuleRegistryUpdater()
        {
            CompilationPipeline.assemblyCompilationFinished += OnScriptsCompiled;
        }

        private static void OnScriptsCompiled(string assemblyPath, CompilerMessage[] messages)
        {
            UpdateBootstrapModuleRegistry();
        }



        public static void UpdateBootstrapModuleRegistry()
        {
            var moduleTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypesSafe())
                .Where(type => type.IsClass
                    && !type.IsAbstract
                    && type.IsSubclassOf(typeof(IAsyncGameBootModule)))
                .Select(type => type.AssemblyQualifiedName)
                .ToList();

            if (!moduleTypes.SequenceEqual(_bootModuleRegistry.ModuleTypeNames))
            {
                _bootModuleRegistry.ModuleTypeNames = moduleTypes;
                EditorBootModulesRegistryProvider.Save();
                EditorUtility.DisplayDialog("Boot modules registry", $"{moduleTypes.Count} modules found.", "OK");
            }
        }
    }
}
