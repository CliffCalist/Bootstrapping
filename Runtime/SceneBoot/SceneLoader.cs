using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        public static IEnumerator LoadScene(string sceneName)
        {
            GameBootModulesRegistryProvider.ThrowIsNotEnabled();
            GameBoot.ThrowIfNotLaunched();

            if (LoadingScreenProvider.Screen != null)
            {
                var isScreenShowed = false;
                LoadingScreenProvider.Screen.Show(() => isScreenShowed = true);

                var waitWhileScreenShowing = new WaitWhile(() => !isScreenShowed);
                yield return waitWhileScreenShowing;
            }

            yield return LoadIntermediateScene();

            Debug.Log($"<color=yellow>Loading target scene: {sceneName}</color>");
            yield return SceneManager.LoadSceneAsync(sceneName);
            Debug.Log($"<color=green>Scene {sceneName} successfully loaded.</color>");

            RunSceneBoot(() => LoadingScreenProvider.Screen?.Hide());
        }


        private static IEnumerator LoadIntermediateScene()
        {
            if (SceneManager.GetActiveScene().name != INTERMEDIATE_SCENE_NAME)
            {
                Debug.Log($"<color=yellow>Loading intermediate scene: {INTERMEDIATE_SCENE_NAME}</color>");
                yield return SceneManager.LoadSceneAsync(INTERMEDIATE_SCENE_NAME);
            }
        }

        private static void RunSceneBoot(Action onCompleted)
        {
            var sceneBootstrap = UnityEngine.Object.FindAnyObjectByType<SceneBoot>();
            if (sceneBootstrap == null)
            {
                Debug.LogWarning($"{nameof(SceneBoot)} isnt found on loaded scene.");
                onCompleted();
            }
            else sceneBootstrap.Run(onCompleted);
        }
    }
}