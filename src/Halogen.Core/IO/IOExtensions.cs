using Spectre.System.IO;
using System.IO;

namespace Halogen.Core.IO
{
    public static class IOExtensions
    {
        internal static byte[] ReadBytes(this IFile file) {
            var stream = file.OpenRead();
            using (MemoryStream ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}