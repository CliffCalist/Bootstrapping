using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WhiteArrow.Bootstraping
{
    public static class GameBoot
    {
        public static bool IsLaunched { get; private set; }


        public static void ThrowIfNotLaunched()
        {
            if (!IsLaunched)
                throw new InvalidOperationException($"Not all {nameof(IGameBootModule)} have finished working.");
        }



        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void OnGameLaunched()
        {
            if (GameBootModulesRegistryProvider.LogIsNotEnabled())
                return;

            Debug.Log("<b>Game is bootstraping...</b>");

            var modules = GameBootModulesRegistryProvider.CreateModules();
            foreach (var module in modules)
            {
                var moduleName = module.GetType().Name;

                Debug.Log($"<color=yellow>Running game module: {moduleName}.</color>");
                module.Run();
                Debug.Log($"<color=green>Game module {moduleName} executed.</color>");
            }

            IsLaunched = true;
            Debug.Log("<color=green>All game modules executed.</color>");

            var loadSceneCoroutine = SceneLoader.LoadScene(SceneManager.GetActiveScene().name);
            Coroutines.Launch(loadSceneCoroutine);
        }
    }
}