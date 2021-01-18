using System;

namespace DemoConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Func<string,int,Models.OjbectCreate.ClassB> funcInitClassB = CommonService.Assembly.CreateInstance<string,int,Models.OjbectCreate.ClassB>();
            //var classA = funcInitClassA();

            var result1 = StopWatcher(() => {
                for (int i = 0; i < 100000; i++)
                {
                    Activator.CreateInstance(typeof(Models.OjbectCreate.ClassB), "Fred", 31);
                }
            });

            Console.WriteLine($"Activator.CreateInstance cost: {result1}");

            var result2 = StopWatcher(() => {
                for (int i = 0; i < 100000; i++)
                {
                    funcInitClassB("Fred",31);
                }
            });

            Console.WriteLine($"Expression CreateInstance cost: {result2}");
            

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
