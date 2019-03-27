using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SuperScaffolding
{
    public class ServiceAnalysis : CSharpCompilationAnalysis
    {
        private const string ServiceCollectionFullTypeName = "Microsoft.Extensions.DependencyInjection.IServiceCollection";

        public override async Task AnalyzeAsync(
            AnalysisContext context,
            CSharpCompilation compilation,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (compilation == null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            var configureServicesMethods = new List<MethodDeclarationSyntax>();
            var results = new List<ServiceDependencyItem>();
            foreach (var syntaxTree in compilation.SyntaxTrees.OfType<CSharpSyntaxTree>().Where(t => IsStartup(t)))
            {
                var methods = await FindMethodDeclarationVisitor.GetMethodsAsync(compilation, syntaxTree, null, "ConfigureServices").ConfigureAwait(false);
                if (methods.Count == 0)
                {
                    continue;
                }

                for (var i = 0; i < methods.Count; i++)
                {
                    var calls = await FindInvocationExpressionVisitor.GetMethodCallsAsync(compilation, syntaxTree, methods[i], ServiceCollectionFullTypeName).ConfigureAwait(false);
                    for (var j = 0; j < calls.Count; j++)
                    {
                        results.Add(new ServiceDependencyItem(compilation, calls[j]));
                    }
                }

                configureServicesMethods.AddRange(methods);

                // Track nodes so we can find them later.
                var root = await syntaxTree.GetRootAsync().ConfigureAwait(false);
                var tracked = root.TrackNodes(methods);
                compilation = compilation.ReplaceSyntaxTree(syntaxTree, syntaxTree.WithRootAndOptions(tracked, syntaxTree.Options));
            }

            context.SetData(new ServiceDependencySet(configureServicesMethods, results));
            context.SetData<CSharpCompilation>(compilation);
        }

        private static bool IsStartup(SyntaxTree syntaxTree)
        {
            return syntaxTree.FilePath?.EndsWith("Startup.cs", StringComparison.OrdinalIgnoreCase) ?? false;
        }
    }
}
