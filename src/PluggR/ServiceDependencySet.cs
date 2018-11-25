using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PluggR
{
    public class ServiceDependencySet : DependencySet<ServiceDependencyItem>
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
    }
}
