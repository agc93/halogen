using System;
using System.Collections.Generic;
using System.Linq;
using Halogen;
using Halogen.Core;
using Halogen.Core.Services;
using Spectre.System.IO;

namespace Halon
{
    public class HalonClient<T> where T: VideoFile

    {
    private MetadataEmbedService _metaService;
    private DirectoryPath _root;
    private IFileSystem _fileSystem;

    public IDataStore<T> Store { get; set; }

    internal HalonClient(string directoryPath, MetadataEmbedService metaService)
    {
        _metaService = metaService;
        _root = new DirectoryPath(directoryPath);
        _fileSystem = metaService.FileSystem ?? new FileSystem();
    }

    private IEnumerable<VideoFile> GetIndex()
    {
        return new List<VideoFile>();
    }

    private List<IFile> GetFiles()
    {
        return _fileSystem
            .GetDirectory(_root)
            .GetFiles("*", SearchScope.Recursive)
            .Where(f => Constants.SupportedExtensions.Contains(f.Path.GetExtension().Name))
            .ToList();
    }

    private List<FilePath> GetFilePaths()
    {
        return GetFiles().Select(f => f.Path).ToList();
    }

    public IEnumerable<VideoFile> Initialize(bool forceExisting = false)
    {
        var allFiles = GetFiles();
        foreach (var file in allFiles)
        {
            if (forceExisting || !_metaService.HasMetadata(file.Path.FullPath))
            {
                var id = _metaService.InitializeFile(file.Path.FullPath);
                yield return new VideoFile {Path = file.Path.FullPath, VideoId = id};
            }
        }
    }

    public IEnumerable<VideoFile> BuildIndex()
    {
        var allFiles = GetFilePaths();
        var emptyFiles = allFiles.Where(f => !_metaService.HasMetadata(f.FullPath));
        var videoEntries = (from emptyFile in emptyFiles
            let id = _metaService.InitializeFile(emptyFile.FullPath)
            select new VideoFile {Path = emptyFile.FullPath, VideoId = id}).ToList();
        videoEntries.AddRange(allFiles.Where(f => _metaService.HasMetadata(f.FullPath)).Select(f =>
            new VideoFile {Path = f.FullPath, VideoId = _metaService.ExtractMetadata(f.FullPath).Id}));

        return videoEntries;
    }

    public IEnumerable<VideoFile> LocateMissingFiles()
    {
        var allFiles = _fileSystem.GetDirectory(_root).GetFiles("*", SearchScope.Recursive).ToList();
        var indexedFiles = GetIndex().ToList();
        var missingFiles = indexedFiles.Where(vf =>
            // ReSharper disable once SimplifyLinqExpression
            !allFiles.Any(f => f.Path.FullPath == _fileSystem.GetFile(new FilePath(vf.Path)).Path.FullPath));
        foreach (var missingFile in missingFiles)
        {
            var missingPath =
                allFiles.FirstOrDefault(af => _metaService.GetId(af.Path.FullPath) == missingFile.VideoId);
            if (missingPath == null) break;
            missingFile.Path = missingPath.Path.FullPath;
            indexedFiles.Replace(missingFile);
        }

        return indexedFiles;
    }
    }
}
