//TODO: This really shouldn't be leaking this badly, this early :(
using System;
using System.Collections.Generic;
//using System.IO;
using System.Linq;
using Halogen.Core.IO;
using Spectre.System.IO;

namespace Halogen.Core.Services
{
    public class MetadataEmbedService
    {
        public IFileSystem FileSystem { get; } = new FileSystem();
        // public IFileAccessProvider FileProvider {get;set;}
        public MetadataEmbedService()
        {
            
        }

        public MetadataEmbedService(IFileSystem fileSystem)
        {
            fileSystem ??= new FileSystem();
        }

        public bool HasMetadata(string filePath)
        {
            var path = new FilePath(filePath);
            return GetMetadata(path, v => v.Tag.RemixedBy != null && v.Tag.RemixedBy.Contains(("x-halid")));
        }

        public HalogenId GetId(string filePath)
        {
            var path = new FilePath(filePath);
            return GetMetadata(path, v => v.Tag.RemixedBy.Split(Halogen.Constants.KeyDelimiter).LastOrDefault());
        }

        // public MetadataEmbedService(IFileSystem _fileSystem, IFileAccessProvider _fileProvider)
        // {
        //     
        // }
        public HalogenId InitializeFile(string filePath) {
            // var mTime = new FileInfo(filePath).LastWriteTime;
            // var tfile = TagLib.File.Create(filePath);
            var path = new FilePath(filePath);
            if (GetMetadata(path, v => v.Tag.RemixedBy, out var existingField)) {
                return existingField.Split(Halogen.Constants.KeyValueDelimiter).Last();
            }
            var fileId = HalogenId.Create(new System.IO.FileInfo(filePath), FileSystem);
            ModifyMetadata(path, f => f.Tag.RemixedBy = $"x-halid{Halogen.Constants.KeyValueDelimiter}{fileId}");
            // tfile.Tag.Conductor = $"x-halid|{fileId}";
            // tfile.Save();
            // File.SetLastWriteTime(filePath, mTime);
            return fileId;
        }

        public (string, string) EmbedKey(string filePath, string key, string value = null) {
            value ??= string.Empty;
            ModifyMetadata(new FilePath(filePath), f => f.AddTag(key, value));
            return (key, value);
        }

        public string SetVideoTitle(string filePath, string title) {
            var finalFile = ModifyMetadata(new FilePath(filePath), f => f.Tag.Title = title);
            var newTitle = TagLib.File.Create(finalFile.FullPath).Tag.Title;
            return newTitle;
        }

        public string GetVideoTitle(string filePath) {
            var file = GetMetadata(new FilePath(filePath), f => f.Tag.Title);
            return file;
        }

        /// <summary>
        /// USE WITH CAUTION: this is not guaranteed to work! 
        /// This method attempts to embed *all* the metadata of a <see cref="Halogen.Core.Video"/> into the original file. 
        /// This is almost certainly against the spec of the file and may actually break it, so back up the original file first.
        /// </summary>
        /// <remarks>
        /// Exceptions are not caught here. You may receive exceptions from underlying implementations so catch generously.
        /// </remarks>
        public virtual void EmbedMetadata(Video video, string videoPath) {
            var finalFile = ModifyMetadata(new FilePath(videoPath), f => f.Tag.Conductor = video.Data);
            finalFile = ModifyMetadata(finalFile, f => f.Tag.Title = video.Title);
            finalFile = ModifyMetadata(finalFile, f => f.Tag.Composers = video.Tags.ToStringList().ToArray());
        }

        public virtual Video ExtractMetadata(string videoPath) {
            // var actions
            var videoFilePath = new FilePath(videoPath);
            var meta = GetMetadataSet(videoFilePath, new List<Func<TagLib.File, string>> {
                f => f.Tag.RemixedBy,
                ((TagLib.File f) => f.Tag.Title),
                f => f.Tag.Conductor
            }).ToList();
            var tags = GetMetadata(videoFilePath, f => f.Tag.Composers);
            return new Video {
                Id = HalogenId.Parse(meta[0]),
                Title = meta[1],
                Data = meta[2],
                Tags = new TagSet(tags)
            };
        }

        private FilePath ModifyMetadata(FilePath filePath, Action<TagLib.File> fileAction) {
            //var file = FileSystem.GetFile(filePath);
            //var mTime = file.LastWriteTime;
            using (var tFile = TagLib.File.Create(filePath.FullPath))
            {
                fileAction.Invoke(tFile);
                tFile.Save();
            }
            // File.SetLastWriteTime(filePath, mTime);
            return filePath;
        }

        private T GetMetadata<T>(FilePath filePath, Func<TagLib.File, T> fileAction) {
            var mTime = FileSystem.GetFile(filePath).LastWriteTime;
            using var tFile = TagLib.File.Create(filePath.FullPath);
            var result = fileAction.Invoke(tFile);
            return result;
        }

        private bool GetMetadata<T>(FilePath filePath, Func<TagLib.File, T> fileAction, out T result) {
            var inFile = GetMetadata(filePath, fileAction);
            result = inFile;
            return result != null && (!(result is string) || !string.IsNullOrWhiteSpace(inFile.ToString()));
        }

        private IEnumerable<T> GetMetadataSet<T>(FilePath filePath, IEnumerable<Func<TagLib.File, T>> fileActions) {
            var mTime = FileSystem.GetFile(filePath).LastWriteTime;
            using var tFile = TagLib.File.Create(filePath.FullPath);
            foreach (var action in fileActions)
            {
                var result = action.Invoke(tFile);
                yield return result;
            }
        }
    }
}