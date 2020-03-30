namespace Halogen.Core
{
    public class VideoFile
    {
        public HalogenId VideoId { get; set; }
        public string Path { get; set; }
        
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            return obj is VideoFile videoFile && Equals(videoFile);
        }

        private bool Equals(VideoFile other)
        {
            return other != null && VideoId.Equals(other.VideoId);
        }
    }
}