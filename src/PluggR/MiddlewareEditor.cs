using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace PluggR
{
    internal class MiddlewareEditor : Editor
    {
        public override async Task ApplyAsync(EditorContext context, Operation operation, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (operation == null)
            {
                throw new ArgumentNullException(nameof(operation));
            }

            if (operation is MiddlewareDependencySet.AddOperation add)
            {
                var compilation = context.GetData<CSharpCompilation>();
                var syntaxTree = FindCorrespondingSyntaxTree(compilation, add.ConfigureMethod.SyntaxTree);

                var root = await syntaxTree.GetRootAsync().ConfigureAwait(false);
                var configureMethod = root.GetCurrentNode(add.ConfigureMethod);
                var edited = syntaxTree.WithRootAndOptions(
                    root.ReplaceNode(
                        configureMethod,
                        configureMethod.AddBodyStatements(
                            ExpressionStatement(
                                InvocationExpression(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName("app"),
                                        IdentifierName("UseMvc")))))),
                    syntaxTree.Options);

                compilation = compilation.ReplaceSyntaxTree(syntaxTree, edited);
                context.SetData<CSharpCompilation>(compilation);
            }
        }

        private static SyntaxTree FindCorrespondingSyntaxTree(CSharpCompilation compilation, SyntaxTree syntaxTree)
        {
            if (compilation.ContainsSyntaxTree(syntaxTree))
            {
                return syntaxTree;
            }

            for (var i = 0; i < compilation.SyntaxTrees.Length; i++)
            {
                if (string.Equals(syntaxTree.FilePath, compilation.SyntaxTrees[i].FilePath, StringComparison.OrdinalIgnoreCase))
                {
                    return (SyntaxTree)compilation.SyntaxTrees[i];
                }
            }

            return null;
        }
    }
}
