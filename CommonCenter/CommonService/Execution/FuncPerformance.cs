using System;
using System.Collections.Generic;
using System.Text;

namespace CommonService.Execution
{
    public class FuncPerformance
    {
        public static long StopWatcher(Action action)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            action();
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
    }
}
