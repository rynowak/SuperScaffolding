using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using PluggR.Plugins;

namespace PluggR
{
    internal class ListCommand : CommandBase
    {
        public ListCommand()
        {
            Project = Option("-p|--project", "project", CommandOptionType.SingleValue);
            HelpOption("-h|--help");

            OnExecute(() => Execute());

            FullName = "PluggR list";
            Name = "list";
            Description = "List possible modificatons for ASP.NET Core application Startup code";
        }

        public CommandOption Project { get; }

        private async Task<int> Execute()
        {
            if (!TryValidateProjectPath(Project, out var projectPath))
            {
                return 1;
            }

            Out.WriteLine("Supported plugins:");
            foreach (var (name, plugin) in KnownPlugins.Plugins)
            {
                Out.WriteLine($"\t{name} ({plugin.GetType().FullName})");
            }

            return 0;
        }
    }
}
