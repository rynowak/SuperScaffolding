using System;
using System.Diagnostics;

namespace Pluggr
{
    internal class Program
    {
        public static readonly string TargetDirectory = @"C:\Users\Ryan\.dotnet\tools\.store\dotnet-aspnet-codegenerator\2.1.1\dotnet-aspnet-codegenerator\2.1.1\tools\netcoreapp2.1\any\";

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
