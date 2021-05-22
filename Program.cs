using System;

namespace C_
{
    class Program
    {
        static void Main(string[] args)
        {
            // MagicNumberTest.Test();
            var start = DateTime.Now;
            var context = new MagicContext();
            var n = 150;
            var result = context.Find(n);
            Console.WriteLine("{0}th number is: {1}", n, result);
            var end = DateTime.Now;

            Console.WriteLine("TimeCost: {0}", end - start);
        }
    }
}
