using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Locality
{
    public static class Extensions
    {
        public static TValue SetDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue)
        {
            TValue result;
            if (!dictionary.TryGetValue(key, out result))
                return dictionary[key] = defaultValue;
            return result;
        }

        public static double ToRadian(this double val)
        {
            return (Math.PI / 180) * val;
        }
    }
}
