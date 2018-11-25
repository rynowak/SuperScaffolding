using System.Collections.Generic;
using PluggR.Plugins;

namespace PluggR
{
    internal static class KnownPlugins
    {
        public static IReadOnlyList<Plugin> Plugins { get; } = new Plugin[]
        {
            new MvcPlugin(),
        };
    }
}
