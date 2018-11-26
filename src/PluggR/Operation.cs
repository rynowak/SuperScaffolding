using System;

namespace PluggR
{
    public abstract class Operation 
    {
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
