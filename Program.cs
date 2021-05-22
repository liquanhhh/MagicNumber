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
            context.match(5000000);
            var end = DateTime.Now;

            Console.WriteLine("TimeCost: {0}", end - start);
        }
    }
}
