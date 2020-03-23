using System;
using System.IO;

namespace Halogen.Core.IO
{

    [Obsolete]
    public class DefaultFileAccessProvider : IFileAccessProvider
    {
        public byte[] ReadBytes(FileInfo file)
        {
            return File.ReadAllBytes(file.FullName);
        }

        public string ReadFile(FileInfo file)
        {
            return File.ReadAllText(file.FullName);
        }

        public FileStream Stream(FileInfo file)
        {
            return file.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite);
        }
    }
}