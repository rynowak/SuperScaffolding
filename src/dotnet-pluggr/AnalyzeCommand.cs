using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.CommandLineUtils;
using PluggR;

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
            var compilation = await GetCompilationAsync(projectPath);
            context.SetData<CSharpCompilation>(compilation);

            Console.WriteLine("Services:");
            foreach (var service in (await context.GetDataAsync<ServiceDependencySet>()).Items)
            {
                Console.WriteLine($"\t{service}");
            }
            Console.WriteLine();

            Console.WriteLine("Middleware:");
            foreach (var middleware in (await context.GetDataAsync<MiddlewareDependencySet>()).Items)
            {
                Console.WriteLine($"\t{middleware.ToString()}");
            }
            Console.WriteLine();
            return 0;
        }
    }
}
