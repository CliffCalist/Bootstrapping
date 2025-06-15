using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace WhiteArrow.Bootstraping
{
    /// <summary>
    /// Handles loading bootstrap modules from the BootstrapModuleRegistry ScriptableObject.
    /// </summary>
    public static class GameBootModulesRegistryProvider
    {
        private static GameBootModulesRegistry s_registry;


        public static bool IsEnabled
        {
            get
            {
                LoadRegistry();
                return s_registry.BootstrapingIsEnabled;
            }
        }



        public static bool LogIsNotEnabled()
        {
            if (!IsEnabled)
            {
                Debug.Log("<color=red>Bootstraping is disabled. Skipping bootstrap logic.</color>");
                return true;
            }
            else return false;
        }

        public static void ThrowIsNotEnabled()
        {
            if (!IsEnabled)
                throw new InvalidOperationException("Bootstraping is disabled.");
        }



        /// <summary>
        /// Create all GameBootstrapModule instances from the registry.
        /// </summary>
        /// <returns>A list of instantiated GameBootstrapModule objects.</returns>
        public static List<IAsyncGameBootModule> CreateModules()
        {
            LoadRegistry();

            var modules = new List<IAsyncGameBootModule>();
            if (s_registry == null)
            {
                Debug.LogError($"{nameof(GameBootModulesRegistry)} not found in Resources folder.");
                return modules;
            }

            foreach (var typeName in s_registry.ModuleTypeNames)
            {
                try
                {
                    var type = Type.GetType(typeName);
                    if (type == null)
                    {
                        Debug.LogError($"Type {typeName} could not be found.");
                        continue;
                    }

                    var moduleInstance = (IAsyncGameBootModule)Activator.CreateInstance(type);
                    modules.Add(moduleInstance);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to create module {typeName}: {ex.Message}");
                }
            }

            modules.Sort((a, b) =>
            {
                var orderA = a.GetType().GetCustomAttribute<GameBootOrderAttribute>()?.Order ?? int.MaxValue;
                var orderB = b.GetType().GetCustomAttribute<GameBootOrderAttribute>()?.Order ?? int.MaxValue;
                return orderA.CompareTo(orderB);
            });

            return modules;
        }



        private static void LoadRegistry()
        {
            if (s_registry == null)
                s_registry = Resources.Load<GameBootModulesRegistry>(GameBootModulesRegistry.FILE_NAME);
        }
    }
}
