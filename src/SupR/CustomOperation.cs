using System.Threading;
using System.Threading.Tasks;

namespace SuperScaffolding
{
    public abstract class CustomOperation : Operation
    {
        public abstract Task ApplyAsync(EditorContext context, CancellationToken cancellationToken = default(CancellationToken));
    }
}
