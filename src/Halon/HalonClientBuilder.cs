using System;
using Halogen.Core;
using Halogen.Core.Services;
using Microsoft.Extensions.DependencyInjection;
using Spectre.System.IO;
// ReSharper disable RedundantTypeArgumentsOfMethod

namespace Halon
{
    public class HalonClientBuilder
    {
        private readonly ServiceCollection _services = new ServiceCollection();

        public HalonClientBuilder()
        {
            _services.AddSingleton<IDataStore<VideoFile>, InMemoryDataStore>();
            _services.AddSingleton<MetadataEmbedService>();
            _services.AddSingleton<IFileSystem, FileSystem>();
        }

        public HalonClient<T> Build<T>(string rootPath) where T : VideoFile
        {
            _services.AddSingleton(p =>
                new HalonClient<T>(rootPath, p.GetRequiredService<MetadataEmbedService>()));
            var provider = _services.BuildServiceProvider();
            return provider.GetRequiredService<HalonClient<T>>();
        }

        public HalonClient<VideoFile> Build(string rootPath)
        {
            return Build<VideoFile>(rootPath);
        }

        public HalonClientBuilder UseDataStore<T, TStore>() where TStore : class, IDataStore<T> where T : VideoFile
        {
            _services.AddSingleton<IDataStore<T>, TStore>();
            return this;
        }

        public HalonClientBuilder UseFileSystem(IFileSystem fileSystem)
        {
            _services.AddSingleton<IFileSystem>(fileSystem);
            return this;
        }

        public HalonClientBuilder UseFileSystem<T>() where T : class, IFileSystem
        {
            _services.AddSingleton<IFileSystem, T>();
            return this;
        }

        public HalonClientBuilder OverrideMetadataService(Func<IServiceProvider, MetadataEmbedService> serviceFactory)
        {
            _services.AddSingleton(serviceFactory);
            return this;
        }
    }
}