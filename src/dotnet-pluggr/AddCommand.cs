using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.ProjectModel;
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

            var analysisContext = Analysis.CreateContext();
            var (compilation, projectContext) = await GetCompilationAsync(projectPath);

            analysisContext.SetData<CSharpCompilation>(compilation);
            analysisContext.SetData<IProjectContext>(projectContext);

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

                operations.AddRange(await plugin.GetOperationsAsync(analysisContext));
            }

            Out.WriteLine("Resolved operations:");
            for (var i = 0; i < operations.Count; i++)
            {
                Out.WriteLine($"\t{operations[i]}");
            }
            Out.WriteLine();

            if (!DryRun.HasValue())
            {
                var editorContext = new EditorContext();
                editorContext.SetData<CSharpCompilation>(await analysisContext.GetDataAsync<CSharpCompilation>().ConfigureAwait(false));
                editorContext.SetData<IProjectContext>(await analysisContext.GetDataAsync<IProjectContext>().ConfigureAwait(false));
                var editor = Editor.Create();

                Out.WriteLine("Performing operations:");
                for (var i = 0; i < operations.Count; i++)
                {
                    Out.WriteLine($"\t{operations[i]}");
                    await editor.ApplyAsync(editorContext, operations[i]);
                }
            }
            Out.WriteLine();
            
            return 0;
        }
    }
}
