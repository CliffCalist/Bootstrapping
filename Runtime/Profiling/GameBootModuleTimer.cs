using System;
using System.Diagnostics;

namespace WhiteArrow.Bootstraping
{
    internal class GameBootModuleTimer
    {
        public readonly string ModuleName;
        private Stopwatch _stopwatch;


        public double DurationMilliseconds => _stopwatch?.Elapsed.TotalMilliseconds ?? 0;



        public GameBootModuleTimer(string moduleName)
        {
            if (string.IsNullOrEmpty(moduleName))
                throw new ArgumentNullException(nameof(moduleName));

            ModuleName = moduleName;
        }



        public void OnLaunched()
        {
            _stopwatch = Stopwatch.StartNew();
        }

        public void OnFinished()
        {
            _stopwatch?.Stop();
        }
    }
}