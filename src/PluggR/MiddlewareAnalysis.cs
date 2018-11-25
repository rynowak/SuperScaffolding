using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace PluggR
{
    public class MiddlewareAnalysis : CSharpCompilationAnalysis
    {
        private const string ApplicationBuilderFullTypeName = "Microsoft.AspNetCore.Builder.IApplicationBuilder";

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

            var results = new List<MiddlewareDependencyItem>();
            foreach (var startup in compilation.SyntaxTrees.OfType<CSharpSyntaxTree>().Where(t => IsStartup(t)))
            {
                var semanticModel = compilation.GetSemanticModel(startup);

                var methods = await FindMethodDeclarationVisitor.GetMethodsAsync(compilation, startup, null, "Configure").ConfigureAwait(false);
                for (var i = 0; i < methods.Count; i++)
                {
                    var calls = await FindInvocationExpressionVisitor.GetMethodCallsAsync(compilation, startup, methods[i], ApplicationBuilderFullTypeName).ConfigureAwait(false);
                    for (var j = 0; j < calls.Count; j++)
                    {
                        results.Add(new MiddlewareDependencyItem(calls[j]));
                    }
                }
            }

            context.SetData(new MiddlewareDependencySet(results));
        }

        private static bool IsStartup(SyntaxTree syntaxTree)
        {
            return syntaxTree.FilePath?.EndsWith("Startup.cs", StringComparison.OrdinalIgnoreCase) ?? false;
        }
    }
}
