
using System.Threading;
using System.Threading.Tasks;

namespace SuperScaffolding
{
    public abstract class Analysis
    {
        public static AnalysisContext CreateContext()
        {
            var context = new AnalysisContext();

            context.SetAnalysis<MiddlewareDependencySet>(new MiddlewareAnalysis());
            context.SetAnalysis<ServiceDependencySet>(new ServiceAnalysis());

            return context;
        }

        public abstract Task AnalyzeAsync(AnalysisContext context, CancellationToken cancellationToken = default(CancellationToken));
    }
}
