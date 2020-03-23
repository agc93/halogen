namespace Halogen.Core.Services
{
    internal static class TagFileExtensions {
        internal static TagLib.File AddTag(this TagLib.File file, string key, string value) {
            file.Tag.Conductor = $"{file.Tag.Conductor}{Constants.KeyDelimiter}{key}{Constants.KeyValueDelimiter}{value}";
            return file;
        }

        internal static TagLib.File RemoveTag(this TagLib.File file, string key) {
            var tags = new TagSet(file.Tag.Conductor);
            if (!tags.ContainsKey(key)) return file;
            tags.Remove(key);
            file.Tag.Conductor = tags.ToString();
            return file;
        }
    }
}