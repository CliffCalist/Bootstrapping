using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WhiteArrow.Bootstraping
{
    internal static class BootProfiler
    {
        private const string LOG_TAG = "[BootProfiler]";



        public static void LogSceneBootTimer(SceneBootTimer timer)
        {
            if (timer == null)
                throw new ArgumentNullException(nameof(timer));

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"{LOG_TAG} {timer.SceneBootName}");
            sb.AppendLine($" ├ Preparing:\t{timer.PreparingMilliseconds,0:F3} ms");
            sb.AppendLine($" └ Initializing:\t{timer.InitializingMilliseconds,0:F3} ms");

            Debug.Log(sb.ToString());
        }



        public static void LogGameBootModuleTimers(List<GameBootModuleTimer> timers)
        {
            if (timers == null)
                throw new ArgumentNullException(nameof(timers));

            var total = timers.Sum(s => s.DurationMilliseconds);
            var avg = timers.Count > 0 ? timers.Average(s => s.DurationMilliseconds) : 0;
            var min = timers.Count > 0 ? timers.Min(s => s.DurationMilliseconds) : 0;
            var max = timers.Count > 0 ? timers.Max(s => s.DurationMilliseconds) : 0;

            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"{LOG_TAG} Game Boot Modules");
            sb.AppendLine($" ├ total:\t\t{total,0:F3} ms");
            sb.AppendLine($" ├ avg:\t\t{avg,0:F3} ms");
            sb.AppendLine($" ├ min:\t\t{min,0:F3} ms");
            sb.AppendLine($" └ max:\t\t{max,0:F3} ms");

            for (int i = 0; i < timers.Count; i++)
            {
                var timer = timers[i];
                sb.AppendLine($" → ({i}) {timer.ModuleName}\t\t— {timer.DurationMilliseconds,0:F3} ms");
            }

            Debug.Log(sb.ToString());
        }
    }
}