using UnityEditor;
using UnityEditor.Compilation;

namespace WhiteArrowEditor.Bootstraping
{
    [InitializeOnLoad]
    public class PreloadSceneCompileChecker
    {
        private PreloadSceneCompileChecker()
        {
            CompilationPipeline.compilationFinished += OnScriptsCompiled;
        }

        private static void OnScriptsCompiled(object value)
        {
            if (!EditorBootSettingsProvider.Settings.BootstrapingIsEnabled)
                return;

            if (!PreloadSceneUtility.IsValid())
                PreloadSceneUtility.ShowFixDialog();
        }
    }
}