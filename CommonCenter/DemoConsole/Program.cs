using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using OpenCVService;

namespace DemoConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var input_path = @"C:\Users\shtr0\Pictures\alita.jpg";
            var output_path = @"C:\Users\shtr0\Pictures\Export\a.jpg";

            using (Image image = new Image(input_path))
            {
                image.Tailoring(9);
                image.Show(Image.ShowType.Changed);
            }
            Console.WriteLine("Completed");

            Console.ReadKey();
        }
    }
}
