using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace WhiteArrow.Bootstraping
{
    public class BootSettings : ScriptableObject, IReadOnlyBootSettings
    {
        [SerializeField] private bool _isEnabled = true;
        [SerializeField, HideInInspector] private List<string> _moduleTypeNames = new();
        [SerializeField, Min(0)] private float _minLoadingScreenTime = 3f;
        [SerializeField] private LoadingScreen _loadingScreen;



        public const string FILE_NAME = "BootSettings";



        public bool IsEnabled
        {
            get => _isEnabled;
            internal set => _isEnabled = value;
        }

        public IReadOnlyList<string> ModuleTypeNames
        {
            get => _moduleTypeNames;
            internal set => _moduleTypeNames = value.ToList();
        }

        public float MinLoadingScreenTime
        {
            get => _minLoadingScreenTime;
            internal set => _minLoadingScreenTime = Mathf.Max(0, value);
        }

        public LoadingScreen LoadingScreen
        {
            get => _loadingScreen;
            internal set => _loadingScreen = value;
        }



        public bool LogIfNotEnabled()
        {
            if (!_isEnabled)
            {
                Debug.Log("<color=red>Bootstraping is disabled. Skipping bootstrap logic.</color>");
                return true;
            }
            else return false;
        }

        public void ThrowIfNotEnabled()
        {
            if (!IsEnabled)
                throw new InvalidOperationException("Bootstraping is disabled.");
        }



        /// <summary>
        /// Create all GameBootstrapModule instances from the registry.
        /// </summary>
        /// <returns>A list of instantiated GameBootstrapModule objects.</returns>
        public List<IAsyncBootModule> CreateModules()
        {
            var modules = new List<IAsyncBootModule>();

            foreach (var typeName in _moduleTypeNames)
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
    }
}