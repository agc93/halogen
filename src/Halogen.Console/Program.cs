using System;
using System.Collections.Generic;
using System.Linq;
using Halogen.Core;
using Halogen.Core.IO;
using Halogen.Core.Services;

namespace Halogen.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            // var targetFilePath = args.First();
            var targetFilePath = System.IO.Path.Combine(Environment.CurrentDirectory, "show2.mp4");
            // var id = HalogenId.Create(new System.IO.FileInfo(targetFilePath));
            
            var service = new MetadataEmbedService() {
                // FileProvider = new DefaultFileAccessProvider()
            };
            var ct = new Video<List<string>>();
            var extracted = service.ExtractMetadata(targetFilePath);
            var data = extracted.GetData();
            var vd = new Video();
            var current = service.GetVideoTitle(targetFilePath);
            var embedId = service.InitializeFile(targetFilePath);
            var vid = new Video {
                Id = embedId,
                Title = "HalogenTest",
                Tags = new TagSet {["tag"] = "value", ["key"] = "value"},
            };
            vid.SaveData(new List<string> {"test", "values", "here"});
            service.EmbedMetadata(vid, targetFilePath);
            System.Console.WriteLine("Hello World!");
        }
    }
}
