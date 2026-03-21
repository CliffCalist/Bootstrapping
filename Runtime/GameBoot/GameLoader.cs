using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace WhiteArrow.Bootstraping
{
    public static class GameLoader
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
            var settings = BootSettingsProvider.Settings;

            if (settings.LogIfNotEnabled())
                return;

            if (settings.IsLogEnabled(LogLevel.Summary))
                Debug.Log("<b>Game is bootstraping...</b>");

            var bootErrorHandler = CreateBootErrorHandler(settings.ErrorHandlerPrefab);

            PrepareLoadingScreen();
            var bootCompleted = await ExecuteModules(bootErrorHandler);

            await SelfDestroyErrorHandler(bootErrorHandler);

            if (!bootCompleted)
                return;

            IsLaunched = true;
            SceneLoader.LoadScene(1);
        }



        private static BootErrorHandler CreateBootErrorHandler(BootErrorHandler prefab)
        {
            if (prefab == null)
                return null;

            var instance = Object.Instantiate(prefab);
            Object.DontDestroyOnLoad(instance.gameObject);
            return instance;
        }

        private static async Task SelfDestroyErrorHandler(BootErrorHandler handler)
        {
            if (handler == null)
                return;

            try
            {
                await handler.SelfDestroyAsync();
            }
            catch (Exception ex)
            {
                Debug.LogError($"<b>Global boot error handler destroy failed:</b> {handler.GetType().Name}");
                Debug.LogException(ex);
            }
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



        private static async Task<bool> ExecuteModules(BootErrorHandler bootErrorHandler)
        {
            var settings = BootSettingsProvider.Settings;
            var modules = settings.Modules.Where(m => m != null);
            var timers = new List<GameBootModuleTimer>();

            foreach (var module in modules)
            {
                var moduleName = module.GetType().Name;
                var shouldStopBoot = false;

                if (settings.IsLogEnabled(LogLevel.Verbose))
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

                    var isCritical = module.FailureAction == BootFailureAction.Stop;
                    var context = new BootErrorContext(moduleName, ex, isCritical);

                    try
                    {
                        await module.OnErrorAsync(ex);
                    }
                    catch (Exception handlerEx)
                    {
                        Debug.LogError($"<b>Game boot module error handler failed:</b> {moduleName}");
                        Debug.LogException(handlerEx);
                    }

                    if (bootErrorHandler != null)
                    {
                        try
                        {
                            await bootErrorHandler.OnErrorAsync(context);
                        }
                        catch (Exception handlerEx)
                        {
                            Debug.LogError($"<b>Global boot error handler failed:</b> {bootErrorHandler.GetType().Name}");
                            Debug.LogException(handlerEx);
                        }
                    }

                    shouldStopBoot = isCritical;
                }
                finally
                {
                    timer.OnFinished();
                }

                if (shouldStopBoot)
                {
                    Debug.LogError($"<b>Game boot stopped by failure policy.</b> Module: {moduleName}");
                    BootProfiler.LogGameBootModuleTimers(timers);
                    return false;
                }

                if (settings.IsLogEnabled(LogLevel.Verbose))
                    Debug.Log($"<color=green>Game module {moduleName} executed.</color>");
            }

            if (settings.IsLogEnabled(LogLevel.Summary))
                Debug.Log("<color=green>All game modules executed.</color>");

            BootProfiler.LogGameBootModuleTimers(timers);
            return true;
        }
    }
}
