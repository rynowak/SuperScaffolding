using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.ProjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PluggR
{
    internal class PackageAnalysis : ProjectContextAnalysis
    {
        public override Task AnalyzeAsync(AnalysisContext context,
                                          IProjectContext projectContext,
                                          CancellationToken cancellationToken = default(CancellationToken))
        {
            var dependencies = new List<PackageDependencyItem>();

            foreach (var dependency in projectContext.PackageDependencies)
            {
                if (dependency.Type == DependencyType.Package)
                {
                    dependencies.Add(new PackageDependencyItem(dependency.Name, dependency.Version));
                }
            }

            context.SetData<PackageDependencySet>(new PackageDependencySet(dependencies));
            return Task.CompletedTask;
        }
    }
}