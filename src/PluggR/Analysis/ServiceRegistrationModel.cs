using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PluggR.Analysis
{
    public class ServiceRegistrationModel
    {
        public ServiceRegistrationModel(InvocationExpressionSyntax expression)
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
