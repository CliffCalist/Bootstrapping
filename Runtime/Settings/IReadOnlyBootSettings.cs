using System.Collections.Generic;

namespace WhiteArrow.Bootstraping
{
    public interface IReadOnlyBootSettings
    {
        bool IsEnabled { get; }
        float MinLoadingScreenTime { get; }
        ILoadingScreen LoadingScreen { get; }



        bool LogIfNotEnabled();
        void ThrowIfNotEnabled();

        List<IAsyncBootModule> CreateModules();
    }
}