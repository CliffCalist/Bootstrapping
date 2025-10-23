using System.Collections.Generic;

namespace WhiteArrow.Bootstraping
{
    public interface IReadOnlyBootSettings
    {
        bool IsEnabled { get; }
        float MinLoadingScreenTime { get; }
        LoadingScreen LoadingScreen { get; }
        IReadOnlyList<AsyncBootModule> Modules { get; }



        bool LogIfNotEnabled();
        void ThrowIfNotEnabled();
    }
}