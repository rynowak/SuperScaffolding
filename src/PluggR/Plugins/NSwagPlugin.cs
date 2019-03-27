using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PluggR.Plugins
{
    public class NSwagPlugin : Plugin
    {
        private const string AddMvcTypeName = "Microsoft.Extensions.DependencyInjection.MvcServiceCollectionExtensions";
        private const string AddMvcMethodName = "AddMvc";

        private const string NSwagPackageName = "NSwag.AspNetCore";
        private const string NSwagPackageVersion = "12.0.13";
        private const string NSwagServiceMethodName = "AddSwagger";
        private const string NSwagDocumentMiddleware = "UseSwagger";
        private const string NSwagUIMiddleware = "UseSwaggerUi3";
        private const string NSwagBuilderExtensions = "Microsoft.AspNetCore.Builder.NSwagApplicationBuilderExtensions";
        private const string NSwagServiceType = "Microsoft.Extensions.DependencyInjection.NSwagServiceCollectionExtensions";

        public override string Name => "nswag";

        public async override Task<IEnumerable<Operation>> GetOperationsAsync(AnalysisContext context, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var operations = new List<Operation>();

            var packages = await context.GetDataAsync<PackageDependencySet>().ConfigureAwait(false);
            operations.Add(await packages.ResolveAsync(NSwagPackageName, NSwagPackageVersion));

            var services = await context.GetDataAsync<ServiceDependencySet>().ConfigureAwait(false);
            //For now add this so that you get APIExplorer and the world doesn't explode
            //when run on an empty app.
            operations.Add(await services.ResolveAsync(AddMvcTypeName, AddMvcMethodName));
            operations.Add(await services.ResolveAsync(NSwagServiceType, NSwagServiceMethodName));

            var middleware = await context.GetDataAsync<MiddlewareDependencySet>().ConfigureAwait(false);
            operations.Add(await middleware.ResolveAsync(NSwagBuilderExtensions, NSwagDocumentMiddleware));
            operations.Add(await middleware.ResolveAsync(NSwagBuilderExtensions, NSwagUIMiddleware));

            return operations;
        }
    }
}
