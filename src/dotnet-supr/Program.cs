using System;
using System.Diagnostics;

namespace SuperScaffolding
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            var application = new Application();
            var result = application.Execute(args);

            if (Debugger.IsAttached)
            {
                Console.WriteLine("Press ENTER to exit...");
                Console.ReadLine();
            }

            return result;
        }
    }
}
