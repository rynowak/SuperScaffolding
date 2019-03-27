using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SuperScaffolding
{
    public class AnalysisContext
    {
        private readonly object _lock;
        private Dictionary<Type, Analysis> _analysis;
        private Dictionary<Type, Task<object>> _data;

        public AnalysisContext()
        {
            _analysis = new Dictionary<Type, Analysis>();
            _data = new Dictionary<Type, Task<object>>();

            _lock = new object();
        }

        public async Task<T> GetDataAsync<T>(CancellationToken cancellationToken = default(CancellationToken)) where T : class
        {
            Task<object> task;
            lock (_lock)
            {
                if (!_data.TryGetValue(typeof(T), out task))
                {
                    if (!_analysis.TryGetValue(typeof(T), out var analysis))
                    {
                        throw new InvalidOperationException($"No analysis was registered for data of type '{typeof(T)}'.");
                    }

                    task = Task.Run(async () =>
                    {
                        await analysis.AnalyzeAsync(this, cancellationToken).ConfigureAwait(false);

                        lock (_lock)
                        {
                            return _data[typeof(T)];
                        }
                    }).Unwrap();
                    _data.Add(typeof(T), task);
                }
            }

            var obj = await task;
            return obj as T;
        }

        public void SetData<T>(T value) where T : class
        {
            lock (_lock)
            {
                _data[typeof(T)] = Task.FromResult<object>(value);
            }
        }

        public void SetAnalysis<T>(Analysis analysis) where T : class
        {
            if (analysis == null)
            {
                throw new ArgumentNullException(nameof(analysis));
            }

            lock (_lock)
            {
                _analysis[typeof(T)] = analysis;
            }
        }

        public void TryAddAnalysis<T>(Analysis analysis) where T : class
        {
            if (analysis == null)
            {
                throw new ArgumentNullException(nameof(analysis));
            }

            lock (_lock)
            {
                if (!_analysis.ContainsKey(typeof(T)))
                {
                    _analysis[typeof(T)] = analysis;
                }
            }
        }
    }
}