using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PluggR.Plugins
{
    public class HealthCheckPlugin : Plugin
    {
        private const string AddHealthChecksTypeName = "Microsoft.Extensions.Diagnostics.HealthChecks";
        private const string AddHealthChecksMethodName = "AddHealthChecks";

        public override string Name => "healthchecks";

        public async override Task<IEnumerable<Operation>> GetOperationsAsync(AnalysisContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var operations = new List<Operation>();

            var services = await context.GetDataAsync<ServiceDependencySet>().ConfigureAwait(false);
            operations.Add(await services.ResolveAsync(AddHealthChecksTypeName, AddHealthChecksMethodName));

            return operations;
        }
    }
}
