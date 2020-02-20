using System.IO;

namespace Halogen.Core.IO
{
    public interface IFileAccessProvider {
        byte[] ReadBytes(FileInfo file);
        string ReadFile(FileInfo file);
        FileStream Stream(FileInfo file);
    }
}