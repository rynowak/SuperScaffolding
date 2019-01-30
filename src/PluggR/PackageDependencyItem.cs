using System;
using System.Collections.Generic;
using System.Text;

namespace PluggR
{
    public class PackageDependencyItem : DependencyItem
    {
        public PackageDependencyItem(string name, string version)
        {
            Name = name;
            Version = version;
        }

        public string Name { get; }
        public string Version { get; }
    }
}
