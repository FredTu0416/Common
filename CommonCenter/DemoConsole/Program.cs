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
            var input_path = @"C:\Users\shtr0\Pictures\gyy.jpg";
            var output_path = @"C:\Users\shtr0\Pictures\Export\a.jpg";

            using (Image image = new Image(input_path))
            {
                image.ReSize(new OpenCvSharp.Size(image.Width / 2, image.Height / 2));
                image.EdgePreservingFilter(OpenCvSharp.EdgePreservingMethods.NormconvFilter,5,0.45f);
                image.Show(Image.ShowType.Changed);
            }
            Console.WriteLine("Completed");

            Console.ReadKey();
        }
    }
}
