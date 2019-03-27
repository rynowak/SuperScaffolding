using System;

namespace SuperScaffolding
{
    public abstract class Operation 
    {
        internal Operation()
        {
        }

        public static Operation CreateEmpty<T>(T item) where T : DependencyItem
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return new Operation<T>.Empty(item);
        }
    }

    public abstract class Operation<T> : Operation where T : DependencyItem
    {
        public Operation()
        {
        }

        internal class Empty : Operation<T>
        {
            public Empty(T item)
            {
                Item = item;
            }

            public T Item { get; }

            public override string ToString()
            {
                return Item.ToString() + " (already done)";
            }
        }
    }
}
