using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using WhiteArrow.GroupedPerformance;
using Object = UnityEngine.Object;

namespace WhiteArrow.Bootstraping
{
    public static class SceneLoader
    {
        private static ILoadingScreen s_loadingScreen;
        private static int s_loadingScreenRefCount = 0;


        public const string INTERMEDIATE_SCENE_NAME = "Preload";



        internal static void SetLoadingScreen(ILoadingScreen screen)
        {
            s_loadingScreen = screen;
        }



        public static void LoadScene(int buildIndex, bool skipShowLoadingScreenAnimations = false)
        {
            Coroutines.Launch(LoadSceneCoroutine(buildIndex, skipShowLoadingScreenAnimations));
        }

        /// <summary>
        /// Coroutine for loading a scene with an intermediate loading scene.
        /// </summary>
        public static IEnumerator LoadSceneCoroutine(int buildIndex, bool skipShowLoadingScreenAnimations = false)
        {
            var scenePath = SceneUtility.GetScenePathByBuildIndex(buildIndex);
            if (string.IsNullOrEmpty(scenePath))
                throw new ArgumentException($"Scene with build index {buildIndex} not found in Build Settings.");

            var sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            return LoadSceneCoroutine(sceneName, skipShowLoadingScreenAnimations);
        }



        public static void LoadScene(string sceneName, bool skipShowLoadingScreenAnimations = false)
        {
            Coroutines.Launch(LoadSceneCoroutine(sceneName, skipShowLoadingScreenAnimations));
        }

        /// <summary>
        /// Coroutine for loading a scene with an intermediate loading scene.
        /// </summary>
        public static IEnumerator LoadSceneCoroutine(string sceneName, bool skipShowLoadingScreenAnimations = false)
        {
            BootSettingsProvider.Settings.ThrowIfNotEnabled();
            GameBoot.ThrowIfNotLaunched();

            yield return TryShowLoadingScreen(skipShowLoadingScreenAnimations);
            var loadingStartTime = Time.time;
            yield return LoadIntermediateScene();

            Debug.Log($"<color=yellow>Loading target scene: {sceneName}</color>");
            yield return SceneManager.LoadSceneAsync(sceneName);
            Debug.Log($"<color=green>Scene {sceneName} successfully loaded.</color>");

            yield return WaitBootFinish();
            yield return TryHideLoadingScreen(loadingStartTime);
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
            var sceneBootstrap = Object.FindAnyObjectByType<SceneBoot>();
            if (sceneBootstrap == null)
            {
                Debug.LogWarning($"{nameof(SceneBoot)} isn't found on loaded scene.");
                yield break;
            }
            else
            {
                var bootstrapName = sceneBootstrap.GetType().Name;
                PerformanceProfiler.StartSimpleSample(bootstrapName);

                sceneBootstrap.Run();
                yield return new WaitWhile(() => !sceneBootstrap.IsFinished);

                PerformanceProfiler.StopSimpleSample(bootstrapName);
                PerformanceProfiler.LogSimpleSample(bootstrapName);
            }
        }



        private static IEnumerator TryShowLoadingScreen(bool skipShowLoadingScreenAnimations)
        {
            if (s_loadingScreen == null)
                yield break;

            if (s_loadingScreenRefCount == 0 && !s_loadingScreen.IsShowed)
            {
                var isScreenShowed = false;
                s_loadingScreen.Show(skipShowLoadingScreenAnimations, () => isScreenShowed = true);

                var waitWhileScreenShowing = new WaitWhile(() => !isScreenShowed);
                yield return waitWhileScreenShowing;
            }

            s_loadingScreenRefCount++;
        }

        private static IEnumerator TryHideLoadingScreen(float showScreenTime)
        {
            if (s_loadingScreen == null || !s_loadingScreen.IsShowed)
                yield break;

            s_loadingScreenRefCount--;

            if (s_loadingScreenRefCount > 0)
                yield break;

            var elapsedTime = Time.time - showScreenTime;
            var remainingLoadingTime = BootSettingsProvider.Settings.MinLoadingScreenTime - elapsedTime;

            if (remainingLoadingTime > 0f)
                yield return new WaitForSecondsRealtime(remainingLoadingTime);

            s_loadingScreen.Hide();
        }
    }
}