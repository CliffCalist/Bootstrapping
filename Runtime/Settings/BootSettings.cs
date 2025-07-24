using System.Collections.Generic;
using UnityEngine;

namespace WhiteArrow.Bootstraping
{
    public class BootSettings : ScriptableObject
    {
        public bool BootstrapingIsEnabled = true;
        public List<string> ModuleTypeNames = new();
        [SerializeField] private InterfaceField<ILoadingScreen> _loadingScreen;



        public const string FILE_NAME = "BootSettings";



        public ILoadingScreen LoadingScreen => _loadingScreen.Value;
    }
}