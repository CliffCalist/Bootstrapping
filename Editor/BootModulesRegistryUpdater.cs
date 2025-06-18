using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using WhiteArrow.Bootstraping;

namespace WhiteArrowEditor.Bootstraping
{
    public class BootModulesRegistryUpdater
    {
        private static BootSettings _settings => EditorBootSettingsProvider.Load();



        private BootModulesRegistryUpdater()
        {
            CompilationPipeline.assemblyCompilationFinished += OnScriptsCompiled;
        }

        private static void OnScriptsCompiled(string assemblyPath, CompilerMessage[] messages)
        {
            UpdateBootModulesRegistry();
        }



        public static void UpdateBootModulesRegistry()
        {
            var moduleTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypesSafe())
                .Where(type => type.IsClass
                    && !type.IsAbstract
                    && type.IsSubclassOf(typeof(IAsyncBootModule)))
                .Select(type => type.AssemblyQualifiedName)
                .ToList();

            if (!moduleTypes.SequenceEqual(_settings.ModuleTypeNames))
            {
                _settings.ModuleTypeNames = moduleTypes;
                EditorBootSettingsProvider.Save();
                EditorUtility.DisplayDialog("Boot modules registry", $"{moduleTypes.Count} modules found.", "OK");
            }
        }
    }
}
