using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.CommandLineUtils;
using PluggR.Plugins;

namespace PluggR
{
    internal class AddCommand : CommandBase
    {
        public AddCommand()
        {
            Plugins = Argument("plugin", "plugins to add", multipleValues: true);
            Project = Option("-p|--project", "project", CommandOptionType.SingleValue);
            DryRun = Option("-d|--dry-run", "print operations but do not edit anything", CommandOptionType.NoValue);
            HelpOption("-h|--help");

            OnExecute(() => Execute());

            FullName = "PluggR add";
            Name = "add";
            Description = "Add to ASP.NET Core application Start";
        }

        public CommandOption DryRun { get; }

        public CommandArgument Plugins { get; }

        public CommandOption Project { get; }

        private async Task<int> Execute()
        {
            if (!TryValidateProjectPath(Project, out var projectPath))
            {
                return 1;
            }

            if (Plugins.Values.Count == 0)
            {
                ShowHelp();
                return 1;
            }

            var context = Analysis.CreateContext();
            var compilation = await GetCompilationAsync(projectPath);
            context.SetData<CSharpCompilation>(compilation);

            var operations = new List<Operation>();
            for (var i = 0; i < Plugins.Values.Count; i++)
            {
                var name = Plugins.Values[i];

                Out.WriteLine($"Processing '{name}'...");

                if (!KnownPlugins.Plugins.TryGetValue(name, out var plugin))
                {
                    Out.WriteLine($"Unknown plugin '{name}'");
                    return 1;
                }

                operations.AddRange(await plugin.GetOperationsAsync(context));
            }

            Out.WriteLine("Resolved operations:");
            for (var i = 0; i < operations.Count; i++)
            {
                Out.WriteLine($"\t{operations[i]}");
            }
            Out.WriteLine();

            if (!DryRun.HasValue())
            {
                Out.WriteLine("Performing operations:");
                for (var i = 0; i < operations.Count; i++)
                {
                    Out.WriteLine($"\t{operations[i]}");
                }
            }
            Out.WriteLine();
            
            return 0;
        }
    }
}
