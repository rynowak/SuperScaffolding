using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SuperScaffolding
{
    internal class FindInvocationExpressionVisitor : CSharpSyntaxWalker
    {
        public static async Task<List<InvocationExpressionSyntax>> GetMethodCallsAsync(
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

            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var visitor = new FindInvocationExpressionVisitor(semanticModel, methodName);
            visitor.Visit(node);

            return visitor.Results;
        }

        private readonly SemanticModel _semanticModel;
        private readonly string _receiverType;

        private FindInvocationExpressionVisitor(SemanticModel semanticModel, string receiverType)
        {
            _semanticModel = semanticModel;
            _receiverType = receiverType;
        }

        public List<InvocationExpressionSyntax> Results { get; } = new List<InvocationExpressionSyntax>();

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            if (node.Expression is MemberAccessExpressionSyntax method &&
                method.Expression is ExpressionSyntax receiver)
            {
                var receiverType = _semanticModel.GetTypeInfo(receiver);
                if (receiverType.ConvertedType.ToDisplayString() == _receiverType)
                {
                    Results.Add(node);
                }
            }

            DefaultVisit(node);
        }
    }
}
