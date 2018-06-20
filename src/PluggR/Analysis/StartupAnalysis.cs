using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PluggR.Analysis
{
    public static class StartupAnalysis
    {
        public static async Task<StartupModel> AnalyzeCompilation(Compilation compilation)
        {
            var startup = compilation.SyntaxTrees
                .Where(t => t.FilePath.EndsWith("Startup.cs"))
                .FirstOrDefault();
            if (startup == null)
            {
                return null;
            }

            var semanticModel =compilation.GetSemanticModel(startup);
            var root = await startup.GetRootAsync().ConfigureAwait(false);
            var visitor = new Visitor(semanticModel);
            visitor.Visit(root);

            return new StartupModel(startup, visitor.Services, visitor.Middleware);
        }

        private class Visitor : CSharpSyntaxWalker
        {
            private readonly SemanticModel _semanticModel;

            public Visitor(SemanticModel semanticModel)
            {
                _semanticModel = semanticModel;
            }

            public List<ServiceRegistrationModel> Services { get; } = new List<ServiceRegistrationModel>();

            public List<MiddlewareRegistrationModel> Middleware { get; } = new List<MiddlewareRegistrationModel>();

            public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
            {
                if (node.Identifier.ToString() == "ConfigureServices")
                {
                    DefaultVisit(node);
                }
                else if (node.Identifier.ToString() == "Configure")
                {
                    DefaultVisit(node);
                }
            }

            public override void VisitInvocationExpression(InvocationExpressionSyntax node)
            {
                if (node.Expression is MemberAccessExpressionSyntax method &&
                    method.Expression is ExpressionSyntax receiver)
                {
                    var receiverType = _semanticModel.GetTypeInfo(receiver);
                    if (receiverType.ConvertedType.ToDisplayString() == "Microsoft.Extensions.DependencyInjection.IServiceCollection")
                    {
                        Services.Add(new ServiceRegistrationModel(node));
                        return;
                    }

                    if (receiverType.ConvertedType.ToDisplayString() == "Microsoft.AspNetCore.Builder.IApplicationBuilder")
                    {
                        Middleware.Add(new MiddlewareRegistrationModel(node));
                        return;
                    }
                }

                DefaultVisit(node);
            }
        }
    }
}
