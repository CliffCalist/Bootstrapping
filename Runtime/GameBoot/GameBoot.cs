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
            if (BootSettingsProvider.Settings.LogIfNotEnabled())
                return;

            Debug.Log("<b>Game is bootstraping...</b>");

            PrepareLoadingScreen();
            await ExecuteModules();
            IsLaunched = true;

            var startSceneName = SceneManager.GetActiveScene().name;
            var loadSceneCoroutine = SceneLoader.LoadScene(startSceneName);
            Coroutines.Launch(loadSceneCoroutine);
        }

        private static void PrepareLoadingScreen()
        {
            if (BootSettingsProvider.Settings.LoadingScreen != null)
            {
                var screenPrefabAsMono = BootSettingsProvider.Settings.LoadingScreen as MonoBehaviour;
                var screen = UnityEngine.Object.Instantiate(screenPrefabAsMono) as ILoadingScreen;

                screen.MarkAsDontDestroyOnLoad();
                LoadingScreenProvider.SetScreen(screen);
                screen.Show(true, null);
            }
        }

        private static async Task ExecuteModules()
        {
            var modules = BootSettingsProvider.Settings.CreateModules();
            Profiler.StartSample("GameBootModules");

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

            Profiler.StopSample("GameBootModules");
            Debug.Log("<color=green>All game modules executed.</color>");
        }
    }
}