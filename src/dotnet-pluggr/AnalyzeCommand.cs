using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.ProjectModel;

namespace PluggR
{
    internal class AnalyzeCommand : CommandBase
    {
        public AnalyzeCommand()
        {
            Project = Option("-p|--project", "project", CommandOptionType.SingleValue);
            HelpOption("-h|--help");

            OnExecute(() => Execute());

            FullName = "PluggR analyze";
            Name = "analyze";
            Description = "Analyze ASP.NET Core application Startup code";
        }

        public CommandOption Project { get; }
        
        private async Task<int> Execute()
        {
            if (!TryValidateProjectPath(Project, out var projectPath))
            {
                return 1;
            }

            var context = Analysis.CreateContext();
            var (compilation, projectContext) = await GetCompilationAsync(projectPath);
            context.SetData<CSharpCompilation>(compilation);
            context.SetData<IProjectContext>(projectContext);
            Out.WriteLine("Services:");
            foreach (var service in (await context.GetDataAsync<ServiceDependencySet>()).Items)
            {
                Out.WriteLine($"\t{service}");
            }
            Out.WriteLine();

            Out.WriteLine("Middleware:");
            foreach (var middleware in (await context.GetDataAsync<MiddlewareDependencySet>()).Items)
            {
                Out.WriteLine($"\t{middleware.ToString()}");
            }
            Out.WriteLine();

            Out.WriteLine("Packages:");
            foreach (var package in (await context.GetDataAsync<PackageDependencySet>()).Items)
            {
                Out.WriteLine($"\t{package.Name} : {package.Version}");
            }

            return 0;
        }
    }
}
