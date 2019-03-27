using System;
using System.Threading;
using System.Threading.Tasks;

namespace SuperScaffolding
{
    public abstract class Editor
    {
        public static Editor Create()
        {
            return new CompositeEditor(new Editor[] 
            {
                new ServiceEditor(),
                new MiddlewareEditor(),
            });
        }

        public abstract Task ApplyAsync(EditorContext context, Operation operation, CancellationToken cancellationToken = default(CancellationToken));

        private class CompositeEditor : Editor
        {
            private Editor[] _inner;

            public CompositeEditor(Editor[] inner)
            {
                _inner = inner;
            }

            public override async Task ApplyAsync(EditorContext context, Operation operation, CancellationToken cancellationToken = default(CancellationToken))
            {
                if (context == null)
                {
                    throw new ArgumentNullException(nameof(context));
                }

                if (operation == null)
                {
                    throw new ArgumentNullException(nameof(operation));
                }

                for (var i = 0; i < _inner.Length; i++)
                {
                    await _inner[i].ApplyAsync(context, operation, cancellationToken);
                }
            }
        }
    }
}
