using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Formatting;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace PluggR
{
    internal class ServiceEditor : Editor
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

            if (operation is ServiceDependencySet.AddOperation add)
            {
                var compilation = context.GetData<CSharpCompilation>();
                var syntaxTree = FindCorrespondingSyntaxTree(compilation, add.ConfigureServicesMethod.SyntaxTree);

                var root = await syntaxTree.GetRootAsync().ConfigureAwait(false);
                var configureServicesMethod = root.GetCurrentNode(add.ConfigureServicesMethod);
                var edited = syntaxTree.WithRootAndOptions(
                    root.ReplaceNode(
                        configureServicesMethod,
                        configureServicesMethod.AddBodyStatements(
                            ExpressionStatement(
                                InvocationExpression(
                                    MemberAccessExpression(
                                        SyntaxKind.SimpleMemberAccessExpression,
                                        IdentifierName("services"),
                                        IdentifierName(add.MethodName)))))),
                    syntaxTree.Options);
                
                File.WriteAllText(edited.FilePath, edited.GetRoot().NormalizeWhitespace().ToString());

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
