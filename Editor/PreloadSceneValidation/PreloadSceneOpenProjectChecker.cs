using UnityEditor;

namespace WhiteArrowEditor.Bootstraping
{
    [InitializeOnLoad]
    public static class PreloadSceneOpenProjectChecker
    {
        static PreloadSceneOpenProjectChecker()
        {
            EditorApplication.update += ValidateIfNeeded;
        }

        private static void ValidateIfNeeded()
        {
            EditorApplication.update -= ValidateIfNeeded;

            if (!EditorBootSettingsProvider.Settings.IsEnabled)
                return;

            if (!PreloadSceneUtility.IsValid())
                PreloadSceneUtility.ShowFixDialog();
        }
    }
}