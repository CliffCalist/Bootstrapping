using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace WhiteArrow.Bootstraping
{
    public static class BootstrapingSettingsProvider
    {
        private static BootstrapingSettings s_settings;


        public static bool IsEnabled
        {
            get
            {
                LoadSettings();
                return s_settings.BootstrapingIsEnabled;
            }
        }

        public static ILoadingScreen LoadingScreen
        {
            get
            {
                LoadSettings();
                return s_settings.LoadingScreen;
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
        public static List<IAsyncBootModule> CreateModules()
        {
            LoadSettings();

            var modules = new List<IAsyncBootModule>();
            if (s_settings == null)
            {
                Debug.LogError($"{nameof(BootstrapingSettings)} not found in Resources folder.");
                return modules;
            }

            foreach (var typeName in s_settings.ModuleTypeNames)
            {
                try
                {
                    var type = Type.GetType(typeName);
                    if (type == null)
                    {
                        Debug.LogError($"Type {typeName} could not be found.");
                        continue;
                    }

                    var moduleInstance = (IAsyncBootModule)Activator.CreateInstance(type);
                    modules.Add(moduleInstance);
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Failed to create module {typeName}: {ex.Message}");
                }
            }

            modules.Sort((a, b) =>
            {
                var orderA = a.GetType().GetCustomAttribute<BootModuleOrderAttribute>()?.Order ?? int.MaxValue;
                var orderB = b.GetType().GetCustomAttribute<BootModuleOrderAttribute>()?.Order ?? int.MaxValue;
                return orderA.CompareTo(orderB);
            });

            return modules;
        }



        private static void LoadSettings()
        {
            if (s_settings == null)
                s_settings = Resources.Load<BootstrapingSettings>(BootstrapingSettings.FILE_NAME);
        }
    }
}
