using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PluggR
{
    public class MiddlewareDependencySet : DependencySet<MiddlewareDependencyItem>
    {
        internal MiddlewareDependencySet(IEnumerable<MethodDeclarationSyntax> configureMethods, IEnumerable<MiddlewareDependencyItem> items)
        {
            if (configureMethods == null)
            {
                throw new ArgumentNullException(nameof(configureMethods));
            }

            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            Items = items.ToArray();
            ConfigureMethods = configureMethods.ToArray();
        }

        public IReadOnlyList<MiddlewareDependencyItem> Items { get; }

        public IReadOnlyList<MethodDeclarationSyntax> ConfigureMethods { get; }

        public Task<Operation> ResolveAsync(string typeName, string methodName)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            if (methodName == null)
            {
                throw new ArgumentNullException(nameof(methodName));
            }

            for (var i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                if (item.MethodSymbol == null)
                {
                    // No symbol = no match
                    continue;
                }

                if (string.Equals(typeName, item.MethodSymbol.ContainingType.ToDisplayString(), StringComparison.Ordinal) &&
                    string.Equals(methodName, item.MethodSymbol.Name, StringComparison.Ordinal))
                {
                    return Task.FromResult(Operation.CreateEmpty(item));
                }
            }

            return Task.FromResult<Operation>(new AddOperation(ConfigureMethods.First(), typeName, methodName));
        }

        internal class AddOperation : Operation<MiddlewareDependencyItem>
        {
            public AddOperation(MethodDeclarationSyntax configureMethod, string typeName, string methodName)
            {
                ConfigureMethod = configureMethod;
                TypeName = typeName;
                MethodName = methodName;
            }

            public MethodDeclarationSyntax ConfigureMethod { get; }

            public string TypeName { get; }

            public string MethodName { get; }

            public override string ToString()
            {
                return $"app.{MethodName}(...) (add to {Path.GetFileName(ConfigureMethod.SyntaxTree.FilePath)})";
            }
        }
    }
}
