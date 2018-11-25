 using Microsoft.Extensions.CommandLineUtils;

namespace PluggR
{
    internal class Application : CommandLineApplication
    {
        public Application()
        {
            Commands.Add(new AddCommand());
            Commands.Add(new AnalyzeCommand());

            HelpOption("-h|--help");

            OnExecute(() => Execute());

            FullName = "PluggR";
            Name = "PluggR";
            Description = "Command line scaffolding for ASP.NET Core Startup";
        }

        private int Execute()
        {
            ShowHelp();
            return 1;
        }
    }
}
