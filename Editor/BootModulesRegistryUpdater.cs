using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;
using WhiteArrow.Bootstraping;

namespace WhiteArrowEditor.Bootstraping
{
    [InitializeOnLoad]
    public class BootModulesRegistryUpdater
    {
        private static BootstrapingSettings _settings => EditorBootSettingsProvider.Load();



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
                    && typeof(IAsyncBootModule).IsAssignableFrom(type))
                .ToList();

            var moduleTypeAssemblyNames = moduleTypes.Select(t => t.AssemblyQualifiedName).ToList();

            if (!moduleTypeAssemblyNames.SequenceEqual(_settings.ModuleTypeNames))
            {
                _settings.ModuleTypeNames = moduleTypeAssemblyNames;
                EditorBootSettingsProvider.Save();

                var moduleTypeNames = moduleTypes.Select(t => t.FullName);
                var message = $"Boot modules registry updated. Types found: {string.Join(", ", moduleTypeNames)}";
                EditorUtility.DisplayDialog("New boot modules found", message, "OK");
            }
        }
    }
}
