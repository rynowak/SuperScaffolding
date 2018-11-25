using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.CommandLineUtils;
using PluggR;
using PluggR.Plugins;

namespace PluggR
{
    internal class AddCommand : CommandBase
    {
        public AddCommand()
        {
            Plugins = Argument("plugin", "Plugins to add", multipleValues: true);
            Project = Option("-p|--project", "project", CommandOptionType.SingleValue);
            HelpOption("-h|--help");

            OnExecute(() => Execute());

            FullName = "PluggR add";
            Name = "add";
            Description = "Add to ASP.NET Core application Start";
        }

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

            for (var i = 0; i < Plugins.Values.Count; i++)
            {
                var plugin = Plugins.Values[i];

                Console.WriteLine($"Processing '{plugin}'...");
                await ExecutePluginAsync(plugin, context);
            }

            Console.WriteLine();
            return 0;
        }

        private bool TrySelectPlugins(
            IReadOnlyList<Plugin> plugins,
            List<string> names,
            out IReadOnlyList<Plugin> matches)
        {
            var results = new List<Plugin>();
            var failures = new List<string>();
            for (var i = 0; i < names.Count; i++)
            {
                var name = names[i];
                var found = false; ;
                for (var j = 0; j < plugins.Count; j++)
                {
                    if (string.Equals(name, plugins[j].Name, StringComparison.OrdinalIgnoreCase))
                    {
                        found = true;
                        results.Add(plugins[j]);
                        break;
                    }
                }

                if (!found)
                {
                    failures.Add(name);
                }
            }

            matches = results;
            return failures.Count == 0;
        }

        private async Task ExecutePluginAsync(string plugin, AnalysisContext context)
        {
            if (plugin != "mvc")
            {
                return;
            }

            var services = await context.GetDataAsync<ServiceDependencySet>();
            var middleware = await context.GetDataAsync<MiddlewareDependencySet>();

            if (services.Items.Any(s => s.MethodName == "AddMvc"))
            {
                Console.WriteLine("MVC services already registered");
            }
            else
            {

            }

            if (middleware.Items.Any(s => s.MethodName == "UseMvc"))
            {
                Console.WriteLine("MVC Middleware already registered");
            }
            else
            {

            }
        }
    }
}
