using System;
using System.IO;
using System.Security.Cryptography;
using Halogen.Core.IO;
using Spectre.System.IO;

namespace Halogen.Core
{
    public class HalogenId {

        public static HalogenId Create(FileInfo file) {
            return Create(file, new DefaultFileAccessProvider());
        }

        public static HalogenId Create(FileInfo file, Spectre.System.IO.IFileSystem fileSystem) {
            // if (provider == null) throw new ArgumentNullException(nameof(provider), "No IFileAccessProvider implementation provided!");
            if (file == null) throw new ArgumentNullException(nameof(file));
            if (!file.Exists) throw new FileNotFoundException("The specified file does not exist", file.FullName);
            var buffer = fileSystem.GetFile(new FilePath(file.FullName)).ReadBytes();
            using (var cryptoProvider = new SHA1CryptoServiceProvider())
            {
                string hash = BitConverter.ToString(cryptoProvider.ComputeHash(buffer));
                var hashGuid = GuidUtility.Create(GuidUtility.HalogenNamespace, hash);
                return hashGuid;
            }
        }

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
        // private HalogenId(string id) {
        //     _id = new Guid(id);
        // }

        private HalogenId(Guid guid) {
            _id = guid;
        }
        private Guid _id;
        private string _hash;

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
            return this._id.ToString("N");
        }
    }
}
