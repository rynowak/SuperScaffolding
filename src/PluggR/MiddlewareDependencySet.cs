using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace PluggR
{
    public class MiddlewareDependencySet : DependencySet<MiddlewareDependencyItem>
    {
        public MiddlewareDependencySet(IEnumerable<MiddlewareDependencyItem> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException(nameof(items));
            }

            Items = new ReadOnlyCollection<MiddlewareDependencyItem>(items.ToArray());
        }

        public ReadOnlyCollection<MiddlewareDependencyItem> Items { get; }
    }
}
