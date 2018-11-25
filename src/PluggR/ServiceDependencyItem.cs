using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PluggR
{
    public class ServiceDependencyItem : DependencyItem
    {
        public ServiceDependencyItem(InvocationExpressionSyntax expression)
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
