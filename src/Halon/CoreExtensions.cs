using System.Collections.Generic;
using System.Linq;
using Halogen.Core;

namespace Halon
{
    public static class CoreExtensions
    {
        internal static IList<VideoFile> Replace(this IList<VideoFile> videoFiles, VideoFile newFile)
        {
            // var videoFiles = collection.ToList();
            var oldItem = videoFiles.FirstOrDefault(vf => vf.VideoId == newFile.VideoId);
            var index = videoFiles.IndexOf(oldItem);
            if (index != -1)
                videoFiles[index] = newFile;
            return videoFiles;
        }
    }
}