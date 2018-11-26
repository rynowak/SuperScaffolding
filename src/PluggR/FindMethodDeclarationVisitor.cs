using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PluggR
{
    internal class FindMethodDeclarationVisitor : CSharpSyntaxWalker
    {
        public static async Task<List<MethodDeclarationSyntax>> GetMethodsAsync(
            CSharpCompilation compilation,
            CSharpSyntaxTree syntaxTree,
            CSharpSyntaxNode node,
            string methodName)
        {
            if (compilation == null)
            {
                throw new ArgumentNullException(nameof(compilation));
            }

            if (syntaxTree == null)
            {
                throw new ArgumentNullException(nameof(syntaxTree));
            }

            if (methodName == null)
            {
                throw new ArgumentNullException(nameof(methodName));
            }

            node = node ?? await syntaxTree.GetRootAsync().ConfigureAwait(false);

            var visitor = new FindMethodDeclarationVisitor(methodName);
            visitor.Visit(node);

            return visitor.Results;
        }

        private readonly string _methodName;

        private FindMethodDeclarationVisitor(string methodName)
        {
            _methodName = methodName;
        }

        public List<MethodDeclarationSyntax> Results { get; } = new List<MethodDeclarationSyntax>();

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            if (node.Identifier.ToFullString() == _methodName)
            {
                Results.Add(node);
            }
        }
    }
}
