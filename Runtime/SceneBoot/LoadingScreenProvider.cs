using System;

namespace WhiteArrow.Bootstraping
{
    internal static class LoadingScreenProvider
    {
        internal static ILoadingScreen Screen { get; private set; }

        internal static void SetScreen(ILoadingScreen screen)
        {
            Screen = screen ?? throw new ArgumentNullException(nameof(screen));
            Screen.MarkAsDontDestroyOnLoad();
        }
    }
}
