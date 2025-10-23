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
    }
}