using System;
using System.Diagnostics;

namespace WhiteArrow.Bootstraping
{
    internal class SceneBootTimer
    {
        public readonly string SceneBootName;
        private Stopwatch _preparingStopwatch;
        private Stopwatch _initializingStopwatch;



        public double TotalMilliseconds => PreparingMilliseconds + InitializingMilliseconds;
        public double PreparingMilliseconds => _preparingStopwatch?.Elapsed.TotalMilliseconds ?? 0;
        public double InitializingMilliseconds => _initializingStopwatch?.Elapsed.TotalMilliseconds ?? 0;



        public SceneBootTimer(string sceneBootName)
        {
            if (string.IsNullOrEmpty(sceneBootName))
                throw new ArgumentNullException(nameof(sceneBootName));

            SceneBootName = sceneBootName;
        }



        public void OnPreparingPhaseLaunched()
        {
            _preparingStopwatch = Stopwatch.StartNew();
        }

        public void OnPreparingPhaseCompleted()
        {
            _preparingStopwatch.Stop();
        }



        public void OnInitializingPhaseLaunched()
        {
            _initializingStopwatch = Stopwatch.StartNew();
        }

        public void OnInitializingPhaseCompleted()
        {
            _initializingStopwatch.Stop();
        }
    }
}