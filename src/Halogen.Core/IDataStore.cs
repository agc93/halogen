using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Halogen.Core
{
    public interface IDataStore<T> where T : VideoFile
    {
        Task<IEnumerable<HalogenId>> AddVideos(params VideoFile[] videos);
         Task<IEnumerable<ICollection>> GetCollections(string pattern = null);
         Task<IEnumerable<T>> GetVideosForCollection(System.Guid collectionId);
         Task<IEnumerable<T>> GetVideos(Func<IEnumerable<T>, IEnumerable<T>> filter = null);
         Task<ICollection> AddCollection(string name, string shortName = null, IEnumerable<HalogenId> videos = null);
         Task<ICollection> AddVideoToCollection(HalogenId id, ICollection collection);
         Task<ICollection> RemoveVideoFromCollection(HalogenId id, ICollection collection);
         Task<ICollection> UpdateCollection(ICollection collection);
         Task<T> UpdateVideo(T video);
    }

    public interface IBackingStore {
        string RootPath {get;set;}
        Task<bool> Initialize(string dbName, string rootPath = null);
        Task<bool> Refresh(string dbName, Func<VideoFile, VideoFile> missingFileAction = null);
    }
}