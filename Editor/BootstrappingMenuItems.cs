using UnityEditor;

namespace WhiteArrowEditor.Bootstraping
{
    public static class BootstrappingMenuItems
    {
        [MenuItem("Tools/WhiteArrow/Bootstraping/Enable", true)]
        private static bool ValidateEnableBootstraping()
        {
            return !EditorBootSettingsProvider.Settings.IsEnabled;
        }

        [MenuItem("Tools/WhiteArrow/Bootstraping/Enable")]
        private static void EnableBootstraping()
        {
            EditorBootSettingsProvider.Settings.IsEnabled = true;
            EditorBootSettingsProvider.Save();
        }



        [MenuItem("Tools/WhiteArrow/Bootstraping/Disable", true)]
        private static bool ValidateDisableBootstraping()
        {
            return EditorBootSettingsProvider.Settings.IsEnabled;
        }

        [MenuItem("Tools/WhiteArrow/Bootstraping/Disable")]
        private static void DisableBootstraping()
        {
            EditorBootSettingsProvider.Settings.IsEnabled = false;
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