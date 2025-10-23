using UnityEditor;

namespace WhiteArrowEditor.Bootstraping
{
    public static class BootstrappingActivation
    {
        [MenuItem("Tools/WhiteArrow/Bootstraping/Enable", true)]
        private static bool ValidateEnable()
        {
            return !EditorBootSettingsProvider.Settings.IsEnabled;
        }

        [MenuItem("Tools/WhiteArrow/Bootstraping/Enable")]
        public static void Enable()
        {
            EditorBootSettingsProvider.Settings.IsEnabled = true;
            EditorBootSettingsProvider.Save();
        }



        [MenuItem("Tools/WhiteArrow/Bootstraping/Disable", true)]
        private static bool ValidateDisable()
        {
            return EditorBootSettingsProvider.Settings.IsEnabled;
        }

        [MenuItem("Tools/WhiteArrow/Bootstraping/Disable")]
        public static void Disable()
        {
            EditorBootSettingsProvider.Settings.IsEnabled = false;
            EditorBootSettingsProvider.Save();
        }
    }
}