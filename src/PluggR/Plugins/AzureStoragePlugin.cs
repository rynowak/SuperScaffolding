using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PluggR.Plugins
{
    internal class AzureStoragePlugin : Plugin
    {
        private const string AddAzureStorageTypeName = "Microsoft.Extensions.DependencyInjection.AzureStorageServiceCollectionExtensions";
        private const string AddAzureStorageMethodName = "AddAzureStorage";

        public override string Name => "azurestorage";

        public override async Task<IEnumerable<Operation>> GetOperationsAsync(AnalysisContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var operations = new List<Operation>();

            var services = await context.GetDataAsync<ServiceDependencySet>().ConfigureAwait(false);
            operations.Add(await services.ResolveAsync(AddAzureStorageTypeName, AddAzureStorageMethodName));

            return operations;
        }
    }
}
