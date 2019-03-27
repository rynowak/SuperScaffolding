using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.Extensions.CommandLineUtils;
using Microsoft.Extensions.ProjectModel;
using Microsoft.VisualStudio.Web.CodeGeneration.Utils;

namespace SuperScaffolding
{
    internal class CommandBase : CommandLineApplication
    {
        protected async Task<CSharpCompilation> GetCompilationAsync(string projectPath)
        {
            Out.WriteLine("Analyzing project...");
            var stopwatch = Stopwatch.StartNew();

            var obj = Path.Combine(Path.GetDirectoryName(projectPath), "obj");
            Directory.CreateDirectory(obj);

            var inject = Path.Combine(obj, $"{Path.GetFileName(projectPath)}.InjectSupR.targets");

            try
            {
                if (!File.Exists(inject))
                {
                    using (var stream = typeof(Program).Assembly.GetManifestResourceStream("SuperScaffolding.InjectSupR.targets"))
                    {
                        using (var file = File.OpenWrite(inject))
                        {
                            stream.CopyTo(file);
                        }
                    }
                }

                var builder = new MsBuildProjectContextBuilder(projectPath, Path.GetDirectoryName(typeof(Program).Assembly.Location));
                var context = builder.Build();

                var workspace = new RoslynWorkspace(context);
                var project = workspace.CurrentSolution.Projects.Single();
                var compilation = (CSharpCompilation)await project.GetCompilationAsync().ConfigureAwait(false);
                Out.WriteLine("Created compilation in: {0}ms", stopwatch.ElapsedMilliseconds);
                Out.WriteLine();

                return compilation;
            }
            finally
            {
                try
                {
                    File.Delete(inject);
                }
                catch
                {
                }
            }
        }

        protected bool TryValidateProjectPath(CommandOption option, out string projectPath)
        {
            projectPath = option.HasValue() ? option.Value() : Directory.GetCurrentDirectory();
            if (Directory.Exists(projectPath))
            {
                projectPath = Path.Combine(Path.GetFullPath(projectPath), Path.GetFileName(projectPath) + ".csproj");
            }

            if (!File.Exists(projectPath))
            {
                Out.WriteLine($"Project file {projectPath} not found.");
                projectPath = null;
                return false;
            }

            return true;
        }
    }
}
