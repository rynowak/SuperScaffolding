using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace PluggR
{
    public class PackageDependencySet : DependencySet<PackageDependencyItem>
    {
        public PackageDependencySet(IEnumerable<PackageDependencyItem> dependencies)
        {
            Items = new ReadOnlyCollection<PackageDependencyItem>(dependencies.ToArray());
        }

        public ReadOnlyCollection<PackageDependencyItem> Items { get; }

        public Task<Operation> ResolveAsync(string packageName, string packageVersion)
        {
            if (packageName == null)
            {
                throw new ArgumentNullException(nameof(packageName));
            }

            if (packageVersion == null)
            {
                throw new ArgumentNullException(nameof(packageVersion));
            }

            for (int i = 0; i < Items.Count; i++)
            {
                var item = Items[i];
                if (string.IsNullOrEmpty(item.Name))
                {
                    continue;
                }

                if (string.Equals(packageName, item.Name, StringComparison.Ordinal))
                {
                    return Task.FromResult(Operation.CreateEmpty(item));
                }
            }

            return Task.FromResult<Operation>(new AddOperation(packageName, packageVersion));
        }
    }

    internal class AddOperation : Operation<ServiceDependencyItem>
    {
        public AddOperation(string packageName, string packageVersion)
        {
            PackageName = packageName;
            PackageVersion = packageVersion;
        }

        public string PackageName { get; }

        public string PackageVersion { get; }

        public override string ToString()
        {
            return $"packages.{PackageName}(...) (add version {PackageVersion})";
        }
    }
}
