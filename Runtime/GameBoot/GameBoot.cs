using System;
using System.Threading.Tasks;
using UnityEngine;
using WhiteArrow.GroupedPerformance;
using Object = UnityEngine.Object;

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

            SceneLoader.LoadScene(1);
        }

        private static void PrepareLoadingScreen()
        {
            if (BootSettingsProvider.Settings.LoadingScreen != null)
            {
                var screen = Object.Instantiate(BootSettingsProvider.Settings.LoadingScreen);
                Object.DontDestroyOnLoad(screen.gameObject);

                SceneLoader.SetLoadingScreen(screen);
                screen.Show(true, null);
            }
        }

        private static async Task ExecuteModules()
        {
            const string PROFILING_GROUP = "Run GameBootModule's";

            var modules = BootSettingsProvider.Settings.CreateModules();

            foreach (var module in modules)
            {
                var moduleName = module.GetType().Name;

                Debug.Log($"<color=yellow>Running game module: {moduleName}.</color>");
                PerformanceProfiler.StartSample(PROFILING_GROUP, moduleName);

                await module.RunAsync();

                PerformanceProfiler.StopSample(PROFILING_GROUP, moduleName);

                Debug.Log($"<color=green>Game module {moduleName} executed.</color>");
            }

            PerformanceProfiler.LogGroup(PROFILING_GROUP);
            Debug.Log("<color=green>All game modules executed.</color>");
        }
    }
}