using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using WhiteArrow.StackedProfiling;

namespace WhiteArrow.Bootstraping
{
    public static class SceneLoader
    {
        public const string INTERMEDIATE_SCENE_NAME = "Intermediate";



        /// <summary>
        /// Coroutine for loading a scene with an intermediate loading scene.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        /// <returns>IEnumerator for coroutine execution.</returns>
        public static IEnumerator LoadScene(string sceneName, bool skipShowLoadingScreenAnimations = false)
        {
            BootSettingsProvider.ThrowIsNotEnabled();
            GameBoot.ThrowIfNotLaunched();

            if (LoadingScreenProvider.Screen != null && !LoadingScreenProvider.Screen.IsShowed)
            {
                if (!skipShowLoadingScreenAnimations)
                {
                    var isScreenShowed = false;
                    LoadingScreenProvider.Screen.Show(false, () => isScreenShowed = true);

                    var waitWhileScreenShowing = new WaitWhile(() => !isScreenShowed);
                    yield return waitWhileScreenShowing;
                }
                else LoadingScreenProvider.Screen.Show(true, null);
            }

            yield return LoadIntermediateScene();

            Debug.Log($"<color=yellow>Loading target scene: {sceneName}</color>");
            yield return SceneManager.LoadSceneAsync(sceneName);
            Debug.Log($"<color=green>Scene {sceneName} successfully loaded.</color>");

            yield return WaitBootFinish();

            if (LoadingScreenProvider.Screen != null && LoadingScreenProvider.Screen.IsShowed)
                LoadingScreenProvider.Screen.Hide();
        }


        private static IEnumerator LoadIntermediateScene()
        {
            if (SceneManager.GetActiveScene().name != INTERMEDIATE_SCENE_NAME)
            {
                Debug.Log($"<color=yellow>Loading intermediate scene: {INTERMEDIATE_SCENE_NAME}</color>");
                yield return SceneManager.LoadSceneAsync(INTERMEDIATE_SCENE_NAME);
            }
        }

        private static IEnumerator WaitBootFinish()
        {
            var sceneBootstrap = UnityEngine.Object.FindAnyObjectByType<SceneBoot>();
            if (sceneBootstrap == null)
            {
                Debug.LogWarning($"{nameof(SceneBoot)} isn't found on loaded scene.");
                yield break;
            }
            else
            {
                var bootstrapName = sceneBootstrap.GetType().Name;
                Profiler.StartSample(bootstrapName);

                sceneBootstrap.Run();
                yield return new WaitWhile(() => !sceneBootstrap.IsFinished);

                Profiler.StopSample(bootstrapName);
                Profiler.LogSample(bootstrapName);
            }
        }
    }
}