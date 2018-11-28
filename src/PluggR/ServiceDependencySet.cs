using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace PluggR
{
    public sealed class ServiceDependencySet : DependencySet<ServiceDependencyItem>
    {
        public ServiceDependencySet(IEnumerable<MethodDeclarationSyntax> configureServicesMethods, IEnumerable<ServiceDependencyItem> items)
        {
            if (configureServicesMethods == null)
            {
                throw new ArgumentNullException(nameof(configureServicesMethods));
            }

            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            Items = new ReadOnlyCollection<ServiceDependencyItem>(items.ToArray());
            ConfigureServicesMethods = configureServicesMethods;
        }

        public ReadOnlyCollection<ServiceDependencyItem> Items { get; }

        public IEnumerable<MethodDeclarationSyntax> ConfigureServicesMethods { get; }

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

            return Task.FromResult<Operation>(new AddOperation(ConfigureServicesMethods.First(), typeName, methodName));
        }

        internal class AddOperation : Operation<ServiceDependencyItem>
        {
            public AddOperation(MethodDeclarationSyntax configureServicesMethod, string typeName, string methodName)
            {
                ConfigureServicesMethod = configureServicesMethod;
                TypeName = typeName;
                MethodName = methodName;
            }

            public MethodDeclarationSyntax ConfigureServicesMethod { get; }

            public string TypeName { get; }

            public string MethodName { get; }

            public override string ToString()
            {
                return $"services.{MethodName}(...) (add to {Path.GetFileName(ConfigureServicesMethod.SyntaxTree.FilePath)})";
            }
        }
    }
}
