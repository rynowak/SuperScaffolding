using System;
using System.Collections.Generic;

namespace PluggR
{
    public class EditorContext
    {
        private readonly object _lock;
        private Dictionary<Type, object> _data;
        private Dictionary<string, string> _files;

        public EditorContext()
        {
            _data = new Dictionary<Type, object>();
            _files = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            _lock = new object();
        }

        public IReadOnlyDictionary<string, string> Files { get; }

        public T GetData<T>() where T : class
        {
            lock (_lock)
            {
                _data.TryGetValue(typeof(T), out var value);
                return value as T;
            }
        }

        public void SetData<T>(T value) where T : class
        {
            lock (_lock)
            {
                _data[typeof(T)] = value;
            }
        }
    }
}
