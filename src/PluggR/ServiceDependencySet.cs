using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace PluggR
{
    public sealed class ServiceDependencySet : DependencySet<ServiceDependencyItem>
    {
        public ServiceDependencySet(IEnumerable<ServiceDependencyItem> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            Items = new ReadOnlyCollection<ServiceDependencyItem>(items.ToArray());
        }

        public ReadOnlyCollection<ServiceDependencyItem> Items { get; }

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

            return Task.FromResult<Operation>(new AddOperation(typeName, methodName));
        }

        internal class AddOperation : Operation<ServiceDependencyItem>
        {
            public AddOperation(string typeName, string methodName)
            {
                TypeName = typeName;
                MethodName = methodName;
            }

            public string TypeName { get; }

            public string MethodName { get; }

            public override string ToString()
            {
                return $"services.{MethodName}(...) (add)";
            }
        }
    }
}
