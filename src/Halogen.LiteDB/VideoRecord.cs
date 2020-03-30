using System.Collections.Generic;
using Halogen.Core;

namespace Halogen.LiteDB
{
    public class VideoRecord : VideoFile
    {
        public Dictionary<string, string> ExtraTags { get; set; }
    }
}