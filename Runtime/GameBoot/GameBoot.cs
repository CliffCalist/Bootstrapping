using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace WhiteArrow.Bootstraping
{
    public static class GameBoot
    {
        public static bool IsLaunched { get; private set; }


        public static void ThrowIfNotLaunched()
        {
            if (!IsLaunched)
                throw new InvalidOperationException($"Not all {nameof(AsyncBootModule)} have finished working.");
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
            var modules = BootSettingsProvider.Settings.Modules.Where(m => m != null);
            var timers = new List<GameBootModuleTimer>();

            foreach (var module in modules)
            {
                var moduleName = module.GetType().Name;

                Debug.Log($"<color=yellow>Running game module: {moduleName}.</color>");

                var timer = new GameBootModuleTimer(moduleName);
                timers.Add(timer);
                timer.OnLaunched();

                try
                {
                    await module.RunAsync();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"<b>Game boot module failed:</b> {moduleName}");
                    Debug.LogException(ex);
                }
                finally
                {
                    timer.OnFinished();
                }

                Debug.Log($"<color=green>Game module {moduleName} executed.</color>");
            }

            Debug.Log("<color=green>All game modules executed.</color>");
            BootProfiler.LogGameBootModuleTimers(timers);
        }
    }
}