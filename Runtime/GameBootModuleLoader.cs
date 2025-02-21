using System;
using System.Collections.Generic;
using UnityEngine;

namespace WhiteArrow.Bootstraping
{
    /// <summary>
    /// Handles loading bootstrap modules from the BootstrapModuleRegistry ScriptableObject.
    /// </summary>
    public static class GameBootModuleLoader
    {
        private static GameBootModuleRegistry _bootModuleRegistry;


        public static bool BoostrapingIsEnabled
        {
            get
            {
                LoadBootModuleRegistry();
                return _bootModuleRegistry.BootstrapingIsEnabled;
            }
        }


        /// <summary>
        /// Loads all GameBootstrapModule instances from the registry.
        /// </summary>
        /// <returns>A list of instantiated GameBootstrapModule objects.</returns>
        public static List<GameBootModule> LoadModules()
        {
            LoadBootModuleRegistry();

            var modules = new List<GameBootModule>();
            if (_bootModuleRegistry == null)
            {
                Debug.LogError($"{nameof(GameBootModuleRegistry)} not found in Resources folder.");
                return modules;
            }

            foreach (var typeName in _bootModuleRegistry.ModuleTypeNames)
            {
                try
                {
                    var type = Type.GetType(typeName);
                    if (type == null)
                    {
                        Debug.LogError($"Type {typeName} could not be found.");
                        continue;
                    }

                    var moduleInstance = (GameBootModule)Activator.CreateInstance(type);
                    modules.Add(moduleInstance);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to create module {typeName}: {ex.Message}");
                }
            }

            return modules;
        }



        private static void LoadBootModuleRegistry()
        {
            if (_bootModuleRegistry == null)
                _bootModuleRegistry = Resources.Load<GameBootModuleRegistry>(GameBootModuleRegistry.FILE_NAME);
        }
    }
}
