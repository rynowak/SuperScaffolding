using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PluggR
{
    public abstract class Plugin
    {
        public abstract string Name { get; }
        
        public abstract Task<IEnumerable<Operation>> GetOperationsAsync(AnalysisContext context, CancellationToken cancellationToken = default(CancellationToken));
    }
}
