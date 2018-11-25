using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PluggR
{
    public class MiddlewareDependencyItem : DependencyItem
    {
        public MiddlewareDependencyItem(InvocationExpressionSyntax expression)
        {
            Expression = expression;
        }

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

        public override string ToString()
        {
            return $"{Expression.Expression.ToString()}(...)";
        }
    }
}
