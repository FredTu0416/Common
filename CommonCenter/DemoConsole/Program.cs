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
            //var input_path = @"C:\Users\shtr0\Pictures\alita.jpg";
            //var output_path = @"C:\Users\shtr0\Pictures\Export\a.jpg";

            //using (Image image = new Image(input_path))
            //{
            //    image.Tailoring(9);
            //    image.Show(Image.ShowType.Changed);
            //}
            //Console.WriteLine("Completed");

            Console.WriteLine((int)FilterTypes.Model);
            Console.WriteLine(FilterTypes.Model.ToString());

            Console.ReadKey();
        }

        public enum FilterTypes
        {
            Color = 0,
            Upholstery = 1,
            Rims = 2,
            Packages = 3,
            Model = 4,
            Price = 5,
            DeliveryDate = 6,
            None
        }
    }
}
