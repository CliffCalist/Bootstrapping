using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using WhiteArrow.StackedProfiling;

namespace WhiteArrow.Bootstraping
{
    public static class GameBoot
    {
        public static bool IsLaunched { get; private set; }


        public static void ThrowIfNotLaunched()
        {
            if (!IsLaunched)
                throw new InvalidOperationException($"Not all {nameof(IAsyncBootModule)} have finished working.");
        }



        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnGameLaunched()
        {
            _ = RunAsync();
        }

        private static async Task RunAsync()
        {
            if (BootstrapingSettingsProvider.LogIsNotEnabled())
                return;

            Debug.Log("<b>Game is bootstraping...</b>");
            PrepareLoadingScreen();

            var modules = BootstrapingSettingsProvider.CreateModules();
            Profiler.StartSample("GameBoot");

            foreach (var module in modules)
            {
                var moduleName = module.GetType().Name;

                Debug.Log($"<color=yellow>Running game module: {moduleName}.</color>");
                Profiler.StartSample(moduleName);

                await module.RunAsync();

                Profiler.StopSample(moduleName);

                Debug.Log($"<color=green>Game module {moduleName} executed.</color>");
                Profiler.LogSample(moduleName);
            }

            Profiler.StopSample("GameBoot");
            IsLaunched = true;

            Debug.Log("<color=green>All game modules executed.</color>");
            Profiler.LogSample("GameBoot");

            var startSceneName = SceneManager.GetActiveScene().name;
            var loadSceneCoroutine = SceneLoader.LoadScene(startSceneName);
            Coroutines.Launch(loadSceneCoroutine);
        }

        private static void PrepareLoadingScreen()
        {
            if (BootstrapingSettingsProvider.LoadingScreen != null)
            {
                var screenPrefabAsMono = BootstrapingSettingsProvider.LoadingScreen as MonoBehaviour;
                var screen = UnityEngine.Object.Instantiate(screenPrefabAsMono) as ILoadingScreen;

                screen.MarkAsDontDestroyOnLoad();
                LoadingScreenProvider.SetScreen(screen);
                screen.Show(true, null);
            }
        }
    }
}