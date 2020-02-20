using System.Collections.Generic;
using System.Linq;

namespace Halogen.Core
{
    public static class CoreExtensions
    {
        internal static T[] ToArray<T>(this IEnumerable<T> s) {
            return s.ToList().ToArray();
        }
    }
}