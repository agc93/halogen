using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Halogen.Core;

namespace Halon
{
    public class InMemoryDataStore : IDataStore<VideoFile>
    {
        private List<VideoFile> Index { get; set; } = new List<VideoFile>();
        private Dictionary<string, List<HalogenId>> Collections { get; set; } = new Dictionary<string, List<HalogenId>>();

        public async Task<IEnumerable<HalogenId>> AddVideos(params VideoFile[] videos)
        {
            Index.AddRange(videos);
            return Index.Select(i => i.VideoId);
        }

        public async Task<IEnumerable<ICollection>> GetCollections(string pattern = null)
        {
            return Collections.Select(c => BasicCollection.Create(c.Key, c.Key, c.Value));
        }

        public Task<IEnumerable<VideoFile>> GetVideosForCollection(Guid collectionId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<VideoFile>> GetVideos(Func<IEnumerable<VideoFile>, IEnumerable<VideoFile>> filter)
        {
            return filter?.Invoke(Index) ?? Index;
        }

        public async Task<ICollection> AddCollection(string name, string shortName = null, IEnumerable<HalogenId> videos = null)
        {
            
            Collections.Add(name, videos.ToList());
            return Collections.FirstOrDefault(c => c.Key == name).ToCollection();
        }

        public async Task<ICollection> AddVideoToCollection(HalogenId id, ICollection collection)
        {
            var match = Collections.FirstOrDefault(c => c.Key == collection.Name);
            if (string.IsNullOrWhiteSpace(match.Key)) return null;
            match.Value.Add(id);
            Collections[match.Key] = match.Value;
            return match.ToCollection();
        }

        public async Task<ICollection> RemoveVideoFromCollection(HalogenId id, ICollection collection)
        {
            var match = Collections.FirstOrDefault(c => c.Key == collection.Name);
            if (string.IsNullOrWhiteSpace(match.Key)) return null;
            match.Value.Remove(id);
            Collections[match.Key] = match.Value;
            return match.ToCollection();   
        }

        public async Task<ICollection> UpdateCollection(ICollection collection)
        {
            var match = Collections.FirstOrDefault(c => c.Key == collection.Name);
            if (string.IsNullOrWhiteSpace(match.Key)) return null;
            Collections[match.Key] = match.Value;
            return match.ToCollection();
        }

        public async Task<VideoFile> UpdateVideo(VideoFile video)
        {
            var match = Index.FirstOrDefault(i => i.VideoId == video.VideoId);
            if (match != null) match.Path = video.Path;
            Index.Replace(match);
            return match;
        }
    }
    
    internal static class StoreExtensions {
        internal static ICollection ToCollection(this KeyValuePair<string, List<HalogenId>> entry)
        {
            var (name, ids) = entry;
            return BasicCollection.Create(name, null, ids);
        }
    }
}