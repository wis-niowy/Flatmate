using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Flatmate.Helpers
{
    class GenericComparer<T> : IEqualityComparer<T>
    {
        public GenericComparer(Func<T, T, bool> equals, Func<T, int> getHashCode)
        {
            this.equals = equals;
            this.getHashCode = getHashCode;
        }

        readonly Func<T, T, bool> equals;
        public bool Equals(T x, T y)
        {
            return equals(x, y);
        }

        readonly Func<T, int> getHashCode;
        public int GetHashCode(T obj)
        {
            return getHashCode(obj);
        }
    }
}
