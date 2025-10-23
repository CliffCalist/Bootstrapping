using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WhiteArrow.Bootstraping
{
    public class BootSettings : ScriptableObject, IReadOnlyBootSettings
    {
        [SerializeField] private bool _isEnabled = true;
        [SerializeField, Min(0)] private float _minLoadingScreenTime = 3f;
        [SerializeField] private LoadingScreen _loadingScreen;
        [SerializeField] private List<AsyncBootModule> _modules;



        public const string FILE_NAME = "BootSettings";



        public bool IsEnabled
        {
            get => _isEnabled;
            internal set => _isEnabled = value;
        }

        public IReadOnlyList<AsyncBootModule> Modules
        {
            get => _modules;
            internal set => _modules = value.ToList();
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



        internal void AddModule(AsyncBootModule module)
        {
            _modules.Add(module);
        }

        internal void RemoveModule(AsyncBootModule module)
        {
            _modules.Remove(module);
        }


        internal void MoveModuleUp(AsyncBootModule module)
        {
            MoveModule(module, _modules.IndexOf(module) - 1);
        }

        internal void MoveModuleDown(AsyncBootModule module)
        {
            MoveModule(module, _modules.IndexOf(module) + 1);
        }

        internal void MoveModule(AsyncBootModule module, int newIndex)
        {
            if (module == null)
                throw new ArgumentNullException(nameof(module));

            var oldIndex = _modules.IndexOf(module);
            if (oldIndex == -1)
                throw new InvalidOperationException("Module is not present in the BootSettings modules list.");

            var maxIndex = Math.Max(0, _modules.Count - 1);
            var target = Math.Max(0, Math.Min(newIndex, maxIndex));

            if (target == oldIndex)
                return;

            _modules.RemoveAt(oldIndex);
            _modules.Insert(target, module);
        }
    }
}