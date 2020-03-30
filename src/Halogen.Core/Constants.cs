using System.Collections.Generic;

namespace Halogen
{
    public static class Constants
    {
        public const string KeyValueDelimiter = "│";
        public const string KeyDelimiter = "║";
        public static IReadOnlyList<string> SupportedExtensions = new List<string>
        {
            "mkv",
            "ogv",
            "avi",
            "mp4",
            "m4p",
            "m4v",
            "mpeg",
            "mpg",
            "mpe",
            "mpv",
            "mpg",
            "m2v",
            "mov"
        }.AsReadOnly();
    }
}