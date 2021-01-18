using System;
using System.Collections.Generic;

namespace DemoConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            CommonService.Excel.Export export = new CommonService.Excel.Export();
            export.Sheet("ClassA_Sheet", new List<Models.OjbectCreate.ClassA>() {
                new Models.OjbectCreate.ClassA(){
                     ID="123",
                      Name="Fred"
                },
                new Models.OjbectCreate.ClassA()
                {
                     ID="456",
                      Name="Amanada"
                },
                new Models.OjbectCreate.ClassA()
                {
                     ID="789",
                      Name="Jason"
                }
            });
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
