using UnityEditor;
using WhiteArrow.Bootstraping;

namespace WhiteArrowEditor.Bootstraping
{
    public static class BootstrappingMenuItems
    {
        private static BootstrapingSettings s_settings => EditorBootSettingsProvider.Settings;



        [MenuItem("Tools/WhiteArrow/Bootstraping/Enable", true)]
        private static bool ValidateEnableBootstraping()
        {
            return !s_settings.BootstrapingIsEnabled;
        }

        [MenuItem("Tools/WhiteArrow/Bootstraping/Enable")]
        private static void EnableBootstraping()
        {
            s_settings.BootstrapingIsEnabled = true;
            EditorBootSettingsProvider.Save();
        }



        [MenuItem("Tools/WhiteArrow/Bootstraping/Disable", true)]
        private static bool ValidateDisableBootstraping()
        {
            return s_settings.BootstrapingIsEnabled;
        }

        [MenuItem("Tools/WhiteArrow/Bootstraping/Disable")]
        private static void DisableBootstraping()
        {
            s_settings.BootstrapingIsEnabled = false;
            EditorBootSettingsProvider.Save();
        }



        [MenuItem("Tools/WhiteArrow/Bootstraping/Update Registry", true)]
        private static bool ValidateUpdateRegistry()
        {
            return EditorApplication.isPlayingOrWillChangePlaymode == false;
        }

        [MenuItem("Tools/WhiteArrow/Bootstraping/Update Registry")]
        private static void UpdateRegistry()
        {
            BootModulesRegistryUpdater.UpdateBootModulesRegistry();
            EditorBootSettingsProvider.Save();
            EditorUtility.DisplayDialog("Success", "Bootstrapping registry updated successfully.", "OK");
        }



        [MenuItem("Tools/WhiteArrow/Bootstraping/Fix Preload Scene Issue", true)]
        private static bool ValidateFixPreloadSceneIssue()
        {
            return !PreloadSceneUtility.IsValid();
        }

        [MenuItem("Tools/WhiteArrow/Bootstraping/Fix Preload Scene Issue")]
        private static void FixPreloadSceneIssue()
        {
            PreloadSceneUtility.FixIssue();
        }
    }
}