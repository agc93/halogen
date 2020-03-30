using System.Threading.Tasks;

namespace Halogen.LiteDB
{
    internal static class DbExtensions {
        internal static Task<T> ToResult<T>(this T o) {
            return Task.FromResult(o);
        }
    }
}