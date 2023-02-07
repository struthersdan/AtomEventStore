using System.Collections.Generic;

namespace Grean.AtomEventStore.UnitTests
{
    public class HashFreeEqualityComparer<T> : IEqualityComparer<T>
    {
        public bool Equals(T x, T y)
        {
            return object.Equals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return 0;
        }
    }
}
