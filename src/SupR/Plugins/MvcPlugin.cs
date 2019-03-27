using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SuperScaffolding.Plugins
{
    public class MvcPlugin : Plugin
    {
        private const string AddMvcTypeName = "Microsoft.Extensions.DependencyInjection.MvcServiceCollectionExtensions";
        private const string AddMvcMethodName = "AddMvc";

        private const string UseMvcTypeName = "Microsoft.AspNetCore.Builder.MvcApplicationBuilderExtensions";
        private const string UseMvcMethodName = "UseMvc";

        public override string Name => "mvc";

        public override async Task<IEnumerable<Operation>> GetOperationsAsync(AnalysisContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var operations = new List<Operation>();

            var services = await context.GetDataAsync<ServiceDependencySet>().ConfigureAwait(false);
            operations.Add(await services.ResolveAsync(AddMvcTypeName, AddMvcMethodName));

            var middleware = await context.GetDataAsync<MiddlewareDependencySet>().ConfigureAwait(false);
            operations.Add(await middleware.ResolveAsync(UseMvcTypeName, UseMvcMethodName));

            return operations;
        }
    }
}
