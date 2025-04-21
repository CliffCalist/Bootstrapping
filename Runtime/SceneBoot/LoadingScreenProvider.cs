using System;

namespace WhiteArrow.Bootstraping
{
    public static class LoadingScreenProvider
    {
        public static ILoadingScreen Screen { get; private set; }

        public static void SetScreen(ILoadingScreen screen)
        {
            Screen = screen ?? throw new ArgumentNullException(nameof(screen));
            Screen.MarkAsDontDestroyOnLoad();
        }
    }
}
