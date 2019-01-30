using Microsoft.CodeAnalysis.CSharp;
using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.ProjectModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PluggR
{
    public abstract class ProjectContextAnalysis : Analysis
    {
        public sealed override async Task AnalyzeAsync(AnalysisContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            var projectContext = await context.GetDataAsync<IProjectContext>(cancellationToken);
            await AnalyzeAsync(context, projectContext, cancellationToken);
        }

        public abstract Task AnalyzeAsync(AnalysisContext context, IProjectContext projectContext, CancellationToken cancellationToken = default(CancellationToken));
    }
}
