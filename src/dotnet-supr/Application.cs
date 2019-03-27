 using Microsoft.Extensions.CommandLineUtils;

namespace SuperScaffolding
{
    internal class Application : CommandLineApplication
    {
        public Application()
        {
            Commands.Add(new AddCommand());
            Commands.Add(new AnalyzeCommand());
            Commands.Add(new ListCommand());

            HelpOption("-h|--help");

            OnExecute(() => Execute());

            FullName = "SupR Scaffolding";
            Name = "SupR Scaffolding";
            Description = "Command line scaffolding for ASP.NET Core Startup";
        }

        private int Execute()
        {
            ShowHelp();
            return 1;
        }
    }
}
