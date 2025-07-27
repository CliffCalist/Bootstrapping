using System;
using System.Threading.Tasks;
using UnityEngine;
using WhiteArrow.GroupedPerformance;

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

            var loadSceneCoroutine = SceneLoader.LoadScene(1);
            Coroutines.Launch(loadSceneCoroutine);
        }

        private static void PrepareLoadingScreen()
        {
            if (BootSettingsProvider.Settings.LoadingScreen != null)
            {
                var monoScreenPrefab = BootSettingsProvider.Settings.LoadingScreen as MonoBehaviour;
                var monoScreenInstance = UnityEngine.Object.Instantiate(monoScreenPrefab);
                var screenInstance = monoScreenInstance as ILoadingScreen;

                UnityEngine.Object.DontDestroyOnLoad(monoScreenInstance.gameObject);
                SceneLoader.SetLoadingScreen(screenInstance);
                screenInstance.Show(true, null);
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