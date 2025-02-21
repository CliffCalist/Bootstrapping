using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using Object = UnityEngine.Object;

namespace WhiteArrow.Bootstraping
{
    public static class GameBoot
    {
        public static bool IsLaunched { get; private set; }
        private static readonly DiContainer s_gameDiContainer = new();
        private static DiContainer s_chachedSceneDiContainer;

        public const string LOADING_SCENE_NAME = "LoadingScene";


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnGameLaucnhed()
        {
            if (!GameBootModuleLoader.BoostrapingIsEnabled)
            {
                Debug.Log("<color=red>GameBoot is disabled. Skipping bootstrap logic.</color>");
                return;
            }

            Debug.Log("<b>Game is bootstraping...</b>");

            var modules = GameBootModuleLoader.LoadModules()
                .OrderBy(m => m.GetType().GetCustomAttribute<GameBootModuleAttribute>().Order)
                .ToList();

            foreach (var module in modules)
            {
                var moduleName = module.GetType().Name;
                Debug.Log($"<color=yellow>Running game module: {moduleName}.</color>");
                module.Run(s_gameDiContainer);
                Debug.Log($"<color=green>Game module {moduleName} executed.</color>");
            }

            IsLaunched = true;
            Debug.Log("<color=green>All game modules executed.</color>");

            var loadSceneCoroutine = LoadScene(SceneManager.GetActiveScene().name);
            Coroutines.Launch(loadSceneCoroutine);
        }



        /// <summary>
        /// Coroutine for loading a scene with an intermediate loading scene.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        /// <returns>IEnumerator for coroutine execution.</returns>
        public static IEnumerator LoadScene(string sceneName)
        {
            if (!IsLaunched)
                throw new InvalidOperationException($"Not all {nameof(GameBootModule)} have finished working.");

            LoadingScreenProvider.Screen?.Open();

            if (SceneManager.GetActiveScene().name != LOADING_SCENE_NAME)
            {
                Debug.Log($"<color=yellow>Loading intermediate scene: {LOADING_SCENE_NAME}</color>");
                yield return SceneManager.LoadSceneAsync(LOADING_SCENE_NAME);
            }

            Debug.Log($"<color=yellow>Loading target scene: {sceneName}</color>");
            yield return SceneManager.LoadSceneAsync(sceneName);

            Debug.Log($"<color=green>Scene {sceneName} successfully loaded.</color>");

            var sceneBootstrap = Object.FindAnyObjectByType<SceneBoot>();
            if (sceneBootstrap == null)
            {
                Debug.LogWarning($"{nameof(SceneBoot)} isnt found on loaded scene.");
                LoadingScreenProvider.Screen?.Close();
            }
            else
            {
                s_chachedSceneDiContainer = new(s_gameDiContainer);
                sceneBootstrap.Run(s_chachedSceneDiContainer, () => LoadingScreenProvider.Screen?.Close());
            }
        }
    }
}