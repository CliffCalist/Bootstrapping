using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace WhiteArrow.Bootstraping
{
    public static class SceneLoader
    {
        private static LoadingScreen s_loadingScreen;
        private static int s_loadingScreenRefCount = 0;


        public const string INTERMEDIATE_SCENE_NAME = "Preload";



        internal static void SetLoadingScreen(LoadingScreen screen)
        {
            s_loadingScreen = screen;
        }



        public static void LoadScene(int buildIndex, bool skipShowLoadingScreenAnimations = false)
        {
            DontDestroyMono.LaunchCoroutine(
                LoadSceneCoroutine(buildIndex, skipShowLoadingScreenAnimations)
            );
        }

        /// <summary>
        /// Coroutine for loading a scene with an intermediate loading scene.
        /// </summary>
        private static IEnumerator LoadSceneCoroutine(int buildIndex, bool skipShowLoadingScreenAnimations = false)
        {
            var scenePath = SceneUtility.GetScenePathByBuildIndex(buildIndex);

            if (string.IsNullOrEmpty(scenePath))
                throw new ArgumentException($"Scene with build index {buildIndex} not found in Build Settings.");

            var sceneName = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            return LoadSceneCoroutine(sceneName, skipShowLoadingScreenAnimations);
        }



        public static void LoadScene(string sceneName, bool skipShowLoadingScreenAnimations = false)
        {
            DontDestroyMono.LaunchCoroutine(
                LoadSceneCoroutine(sceneName, skipShowLoadingScreenAnimations)
            );
        }

        /// <summary>
        /// Coroutine for loading a scene with an intermediate loading scene.
        /// </summary>
        private static IEnumerator LoadSceneCoroutine(string sceneName, bool skipShowLoadingScreenAnimations = false)
        {
            var settings = BootSettingsProvider.Settings;
            settings.ThrowIfNotEnabled();
            GameLoader.ThrowIfNotLaunched();

            yield return TryShowLoadingScreen(skipShowLoadingScreenAnimations);
            var loadingStartTime = Time.time;
            yield return LoadIntermediateScene();

            if (settings.IsLogEnabled(LogLevel.Summary))
                Debug.Log($"<color=yellow>Loading target scene: {sceneName}</color>");

            yield return SceneManager.LoadSceneAsync(sceneName);

            var sceneBootstrap = Object.FindAnyObjectByType<SceneBoot>();
            if (sceneBootstrap != null)
            {
                var timer = new SceneBootTimer(sceneName);

                timer.OnPreparingPhaseLaunched();
                var preparingTask = sceneBootstrap.PrepareSceneAsync();
                yield return new WaitWhile(() => !preparingTask.IsCompleted);
                timer.OnPreparingPhaseCompleted();

                yield return WaitLeftLoadingScreenTime(loadingStartTime);

                timer.OnInitializingPhaseLaunched();
                var initializationTask = sceneBootstrap.InitializeSceneAsync();
                yield return new WaitWhile(() => !initializationTask.IsCompleted);
                timer.OnInitializingPhaseCompleted();

                BootProfiler.LogSceneBootTimer(timer);
            }
            else
            {
                if (settings.IsLogEnabled(LogLevel.Summary))
                    Debug.LogWarning($"{nameof(SceneBoot)} isn't found on loaded scene.");
            }

            if (settings.IsLogEnabled(LogLevel.Summary))
                Debug.Log($"<color=green>Scene {sceneName} successfully loaded.</color>");

            TryHideLoadingScreen();
        }



        private static IEnumerator LoadIntermediateScene()
        {
            if (SceneManager.GetActiveScene().name != INTERMEDIATE_SCENE_NAME)
            {
                if (BootSettingsProvider.Settings.IsLogEnabled(LogLevel.Verbose))
                    Debug.Log($"<color=yellow>Loading intermediate scene: {INTERMEDIATE_SCENE_NAME}</color>");

                yield return SceneManager.LoadSceneAsync(INTERMEDIATE_SCENE_NAME);
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

        private static IEnumerator WaitLeftLoadingScreenTime(float showScreenTime)
        {
            if (s_loadingScreen == null || !s_loadingScreen.IsShowed)
                yield break;

            var elapsedTime = Time.time - showScreenTime;
            var remainingLoadingTime = BootSettingsProvider.Settings.MinLoadingScreenTime - elapsedTime;

            if (remainingLoadingTime > 0f)
                yield return new WaitForSecondsRealtime(remainingLoadingTime);
        }

        private static void TryHideLoadingScreen()
        {
            if (s_loadingScreen == null || !s_loadingScreen.IsShowed)
                return;

            s_loadingScreenRefCount--;

            if (s_loadingScreenRefCount > 0)
                return;

            s_loadingScreen.Hide();
        }
    }
}
