using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Halogen.Core;
using LiteDB;

namespace Halogen.LiteDB
{
    public class LiteDBStore : IDataStore, IBackingStore
    {
        public LiteDBStore()
        {
            string GetCollectionName(Type t) {
                return $"{t.Name.ToLower()}s";
            }
            BsonMapper.Global.UseCamelCase();
            BsonMapper.Global.ResolveCollectionName = GetCollectionName;
            BsonMapper.Global.RegisterType<HalogenId>(
                serialize: (hid => hid.ToString()),
                deserialize: (bson => (HalogenId)bson.AsString)
            );
            BsonMapper.Global.Entity<VideoFile>().Id(x => x.VideoId);
        }
        private LiteDatabase GetDatabase(bool local = false) {
            (var dbPath, var localPath) = GetPaths();
            var db = new LiteDatabase(local ? localPath : dbPath);
            return db;
        }

        public Task<ICollection> AddCollection(string name, string shortName = null, IEnumerable<HalogenId> videos = null)
        {
            var collection = BasicCollection.Create(name, shortName, videos);
            using (var db = GetDatabase())
            {
                var collections = db.GetCollection<ICollection>("collections");
                collections.Insert(collection);
                collections.EnsureIndex(x => x.Name);
            }
            return Task.FromResult(collection as ICollection);
        }

        public Task<ICollection> AddVideoToCollection(HalogenId id, ICollection collection)
        {
            using (var db = GetDatabase())
            {
                var collections = db.GetCollection<ICollection>("collections");
                var match = collections.FindOne(c => c.Id == collection.Id);
                match.Videos.Add(id);
                collections.Update(match);
                return Task.FromResult(match as ICollection);
            }
        }

        public Task<IEnumerable<ICollection>> GetCollections(string pattern = null)
        {
            using (var db = GetDatabase())
            {
                var collections = db.GetCollection<ICollection>("collections");
                var results = collections.FindAll();
                return Task.FromResult(results);
            }
        }

        public Task<IEnumerable<Video>> GetVideos(Func<IEnumerable<Video>, IEnumerable<Video>> filter = null)
        {
            using (var db = GetDatabase())
            {
                var collections = db.GetCollection<Video>("videos");
                return Task.FromResult(collections.FindAll());
            }
        }

        public Task<IEnumerable<Video>> GetVideosForCollection(Guid collectionId)
        {
            using (var db = GetDatabase())
            {
                var collections = db.GetCollection<ICollection>("collections").FindAll();
                var videos = db.GetCollection<Video>("vidoes").FindAll();
                return Task.FromResult(videos.Select(v => {
                    v.CollectionName = collections.FirstOrDefault(c => c.Videos.Contains(v.Id))?.Name;
                    return v;
                }));
            }
        }

        public Task<ICollection> RemoveVideoFromCollection(HalogenId id, ICollection collection)
        {
            using (var db = GetDatabase())
            {
                var collections = db.GetCollection<ICollection>("collections");
                var match = collections.FindById(collection.Id);
                var changed = match.Videos.RemoveIfPresent(id);
                if (changed) {
                    collections.Update(match);
                }
                return match.ToResult();
            }
        }

        public Task<ICollection> UpdateCollection(ICollection collection)
        {
            using (var db = GetDatabase())
            {
                var collections = db.GetCollection<ICollection>("collections");
                collections.Upsert(collection);
                return collections.FindById(collection.Id).ToResult();
            }
        }

        public Task<Video> UpdateVideo(Video video)
        {
            using (var db = GetDatabase())
            {
                var collections = db.GetCollection<Video>("videos");
                collections.Upsert(video);
                return collections.FindById(video.Id.ToString()).ToResult();
            }
        }

#region Backing Store
        public string RootPath {get;set;}
        private string DatabaseName {get;set;}
        private (string dbPath, string localPath) GetPaths() {
            var dbPath = System.IO.Path.Combine(this.RootPath, $"{DatabaseName}.db");
            var localPath = System.IO.Path.Combine(this.RootPath, $"{DatabaseName}.local");
            return (dbPath, localPath);
        }
        public Task<bool> Initialize(string dbName, string rootPath = null)
        {
            DatabaseName = dbName;
            RootPath = rootPath ?? Environment.CurrentDirectory;
            (var dbPath, var localPath) = GetPaths();
            using (var db = new LiteDatabase(dbPath))
            {
                db.GetCollection<ICollection>("collections");
                db.GetCollection<Video>("videos");
                db.Shrink();
            }
            using (var local = new LiteDatabase(localPath))
            {
                local.GetCollection<VideoFile>("files");
                local.GetCollection<Dictionary<string, string>>("options");
                local.Shrink();
            }
            return Task.FromResult(System.IO.File.Exists(dbPath) && System.IO.File.Exists(localPath));
        }

        public Task<bool> Refresh(string dbName, Func<VideoFile, VideoFile> missingFileAction = null)
        {
            var localPath = System.IO.Path.Combine(RootPath, $"{dbName}.local");
            // IEnumerable<string> allFiles = null;
            using (var db = new LiteDatabase(localPath))
            {
                var collection = db.GetCollection<VideoFile>("files");
                collection.EnsureIndex((vf => vf.VideoId));
                var files = collection.FindAll();
                foreach (var file in files)
                {
                    if (System.IO.File.Exists(Path.Combine(RootPath, file.Path))) continue;
                    if (missingFileAction != null) {
                        var replacement = missingFileAction.Invoke(file);
                        if (replacement != null) {
                            if (replacement.VideoId == file.VideoId) {
                                file.Path = replacement.Path;
                                collection.Update(file);
                            } else {
                                collection.Delete((v => v.VideoId == file.VideoId));
                                collection.Insert(replacement);
                            }
                        }
                    } else {
                        collection.Delete((v => v.VideoId == file.VideoId));
                    }
                }
            }
            return Task.FromResult(false);
        }
#endregion
    }

    internal static class DbExtensions {
        internal static bool RemoveIfPresent<T>(this List<T> list, T target) {
            return list.Contains(target) ? list.Remove(target) : false;
        }

        internal static Task<T> ToResult<T>(this T o) {
            return Task.FromResult(o);
        }
    }
}
