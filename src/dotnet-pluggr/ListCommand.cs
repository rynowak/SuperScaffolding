using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.CommandLineUtils;

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

            var context = Analysis.CreateContext();
            var compilation = await GetCompilationAsync(projectPath);
            context.SetData<CSharpCompilation>(compilation);

            return 0;
        }
    }
}
