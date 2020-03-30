using System;
using System.IO;
using System.Security.Cryptography;
using Halogen.Core.IO;
using Spectre.System.IO;

namespace Halogen.Core
{
    public sealed class HalogenId {
        public static HalogenId Create(FileInfo filePath, IFileSystem fileSystem) {
            // if (provider == null) throw new ArgumentNullException(nameof(provider), "No IFileAccessProvider implementation provided!");
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));
            if (!filePath.Exists) throw new FileNotFoundException("The specified file does not exist", filePath.FullName);
            fileSystem ??= new FileSystem();
            var file = fileSystem.GetFile(new FilePath(filePath.FullName));
            var buffer = new byte[file.Length];
            using var fs = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read) as FileStream;
            fs?.Read(buffer, 0, Convert.ToInt32(file.Length));
            using var cryptoProvider = new SHA1CryptoServiceProvider();
            var hash = BitConverter.ToString(cryptoProvider.ComputeHash(buffer));
            var hashGuid = GuidUtility.Create(GuidUtility.HalogenNamespace, hash);
            return hashGuid;
        }

        [Obsolete]
        public static HalogenId Create(FileInfo file, IFileAccessProvider provider) {
            if (provider == null) throw new ArgumentNullException(nameof(provider), "No IFileAccessProvider implementation provided!");
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (!file.Exists) throw new FileNotFoundException("The specified file does not exist", file.FullName);
            var buffer = provider.ReadBytes(file);
            using (var cryptoProvider = new SHA1CryptoServiceProvider())
            {
                string hash = BitConverter.ToString(cryptoProvider.ComputeHash(buffer));
                var hashGuid = GuidUtility.Create(GuidUtility.HalogenNamespace, hash);
                return hashGuid;
            }
        }

        public static HalogenId Parse(string tagValue) {
            var id = tagValue.Split(Halogen.Constants.KeyValueDelimiter, StringSplitOptions.RemoveEmptyEntries)[1];
            return id;
        }

        private HalogenId(Guid guid) {
            _id = guid;
        }
        private Guid _id;

        public static implicit operator HalogenId(Guid id) {
            return new HalogenId(id);
        }
        public static implicit operator string(HalogenId hid) {
            return hid._id.ToString();
        }

        public static implicit operator HalogenId(string s) {
            return new HalogenId(new Guid(s));
        }

        public static implicit operator Guid(HalogenId hid) {
            return hid._id;
        }

        public Guid ToGuid() {
            return this._id;
        }

        public override string ToString() {
            return this._id.ToString("N").ToUpper();
        }

        private bool Equals(HalogenId other)
        {
            return _id.Equals(other._id);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj.GetType() == this.GetType() && Equals((HalogenId) obj);
        }
    }
}
