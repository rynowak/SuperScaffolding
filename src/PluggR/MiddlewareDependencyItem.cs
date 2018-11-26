using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PluggR
{
    public class MiddlewareDependencyItem : DependencyItem
    {
        public MiddlewareDependencyItem(CSharpCompilation compilation, InvocationExpressionSyntax expression)
        {
            Compilation = compilation;
            Expression = expression;
        }

        public CSharpCompilation Compilation { get; }

        public InvocationExpressionSyntax Expression { get; }

        public string MethodName
        {
            get
            {
                if (Expression.Expression is MemberAccessExpressionSyntax member)
                {
                    return member.Name.ToString();
                }

                return null;
            }
        }

        public ISymbol MethodSymbol
        {
            get
            {
                if (Expression.Expression is MemberAccessExpressionSyntax member)
                {
                    var semanticModel = Compilation.GetSemanticModel(Expression.SyntaxTree, ignoreAccessibility: false);
                    var symbolInfo = semanticModel.GetSymbolInfo(Expression);
                    return symbolInfo.Symbol;
                }

                return null;
            }
        }

        public override string ToString()
        {
            return $"{Expression.Expression.ToString()}(...)";
        }
    }
}
