using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.ProjectModel;
using Microsoft.VisualStudio.Web.CodeGeneration.Utils;
using PluggR.Analysis;

namespace Pluggr
{
    internal class AnalyzeCommand : CommandLineApplication
    {
        public AnalyzeCommand()
        {
            Name = "analyze";
            Project = Option("-p|--project", "project", CommandOptionType.SingleValue);

            OnExecute(() => Execute());
        }

        public CommandOption Project { get; }

        private async Task<int> Execute()
        {
            var projectPath = Project.HasValue() ? Project.Value() : Directory.GetCurrentDirectory();
            if (Directory.Exists(projectPath))
            {
                projectPath = Path.Combine(Path.GetFullPath(projectPath), Path.GetFileName(projectPath) + ".csproj");
            }

            if (!File.Exists(projectPath))
            {
                Console.WriteLine($"Project file {projectPath} not found.");
                return 1;
            }

            Console.WriteLine("Analyzing project...");
            var builder = new MsBuildProjectContextBuilder(projectPath, Program.TargetDirectory);
            var context = builder.Build();

            var workspace = new RoslynWorkspace(context);
            var project = workspace.CurrentSolution.Projects.Single();
            var compilation = await project.GetCompilationAsync().ConfigureAwait(false);

            var startup = await StartupAnalysis.AnalyzeCompilation(compilation).ConfigureAwait(false);

            Console.WriteLine("Found services:");
            foreach (var service in startup.Services)
            {
                Console.WriteLine($"{service}");
            }
            Console.WriteLine();

            Console.WriteLine("Found middleware:");
            foreach (var middleware in startup.Middleware)
            {
                Console.WriteLine($"{middleware.ToString()}");
            }
            Console.WriteLine();
            return 0;
        }
    }
}
