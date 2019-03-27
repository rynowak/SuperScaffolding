using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;

namespace SuperScaffolding
{
    public abstract class CSharpCompilationAnalysis : Analysis
    {
        public sealed override async Task AnalyzeAsync(AnalysisContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            var compilation = await context.GetDataAsync<CSharpCompilation>(cancellationToken);
            await AnalyzeAsync(context, compilation, cancellationToken);
        }

        public abstract Task AnalyzeAsync(AnalysisContext context, CSharpCompilation compilation, CancellationToken cancellationToken = default(CancellationToken));
    }
}
