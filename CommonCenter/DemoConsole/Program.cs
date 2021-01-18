using System;
using System.Collections.Generic;

namespace DemoConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ReadKey();
        }


        static long StopWatcher(Action action)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();
            action();
            stopwatch.Stop();
            return stopwatch.ElapsedMilliseconds;
        }
    }
}
