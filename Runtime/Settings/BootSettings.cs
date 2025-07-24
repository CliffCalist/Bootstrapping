using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WhiteArrow.Bootstraping
{
    public class BootSettings : ScriptableObject
    {
        [SerializeField] private bool _isEnabled = true;
        [SerializeField] private List<string> _moduleTypeNames = new();
        [SerializeField] private InterfaceField<ILoadingScreen> _loadingScreen;



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

        public ILoadingScreen LoadingScreen
        {
            get => _loadingScreen.Value;
            internal set => _loadingScreen.Value = value;
        }
    }
}