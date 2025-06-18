using UnityEditor;
using WhiteArrow.Bootstraping;

namespace WhiteArrowEditor.Bootstraping
{
    public static class BootstrappingMenuItems
    {
        private static GameBootModulesRegistry _bootModuleRegistry => EditorBootModulesRegistryProvider.Load();



        [MenuItem("Tools/WhiteArrow/Bootstraping/Enable", true)]
        private static bool ValidateEnableBootstraping()
        {
            return !_bootModuleRegistry.BootstrapingIsEnabled;
        }

        [MenuItem("Tools/WhiteArrow/Bootstraping/Enable")]
        private static void EnableBootstraping()
        {
            _bootModuleRegistry.BootstrapingIsEnabled = true;
            EditorBootModulesRegistryProvider.Save();
        }



        [MenuItem("Tools/WhiteArrow/Bootstraping/Disable", true)]
        private static bool ValidateDisableBootstraping()
        {
            return _bootModuleRegistry.BootstrapingIsEnabled;
        }

        [MenuItem("Tools/WhiteArrow/Bootstraping/Disable")]
        private static void DisableBootstraping()
        {
            _bootModuleRegistry.BootstrapingIsEnabled = false;
            EditorBootModulesRegistryProvider.Save();
        }
    }
}