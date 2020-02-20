using System.Collections.Generic;

namespace Halogen.Core
{
    public class Video
    {
        public HalogenId Id { get; set; }
        public string Title { get; set; }
        public IList<string> Tags { get; set; }

        internal string Data {get;set;} = "";

        public T GetData<T>() {
            return System.Text.Json.JsonSerializer.Deserialize<T>(Data, new System.Text.Json.JsonSerializerOptions { AllowTrailingCommas = true});
        }

        public void SaveData<T>(T data) {
            Data = System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions { AllowTrailingCommas = true});
        }

        /// <summary>
        /// This is not guaranteed to be set! Provides the name of the collection this video belongs to.
        /// If the video is in multiple collections, this will be the last collection it was added to.
        /// </summary>
        /// <value></value>
        public string CollectionName { get; set; }
    }
}