using System;
using System.Collections.Generic;

namespace DemoConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            CommonService.Excel.Analysis analysis = new CommonService.Excel.Analysis(@"D:\Resource\Import_offline_orders_template(2).xlsx");
            List<Models.OjbectCreate.OrderEntity> entities = analysis.Sheet<Models.OjbectCreate.OrderEntity>("order");
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
