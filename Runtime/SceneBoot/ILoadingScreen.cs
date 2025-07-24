using System;

namespace WhiteArrow.Bootstraping
{
    public interface ILoadingScreen
    {
        bool IsShowed { get; }

        void Show(bool skipAnimations, Action callback);
        void Hide();
    }
}
