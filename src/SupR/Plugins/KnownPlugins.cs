using System;
using System.Collections.Generic;

namespace SuperScaffolding.Plugins
{
    public static class KnownPlugins
    {
        public static IReadOnlyDictionary<string, Plugin> Plugins { get; }

        static KnownPlugins()
        {
            var plugins = new Dictionary<string, Plugin>(StringComparer.OrdinalIgnoreCase);
            Plugins = plugins;

            Add(new AzureStoragePlugin());
            Add(new MvcPlugin());

            void Add(Plugin plugin)
            {
                plugins[plugin.Name] = plugin;
            }
        }
    }
}
