using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace WhiteArrowEditor.Bootstraping
{
    public class PreloadSceneBuildChecker : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (!EditorBootSettingsProvider.Settings.BootstrapingIsEnabled)
                return;

            if (!PreloadSceneUtility.IsValid())
            {
                throw new BuildFailedException(
                    "Missing required Preload scene in Build Settings. Add or generate it before building.");
            }
        }
    }
}