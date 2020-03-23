using System.Collections.Generic;

namespace Halogen.Core
{
    public abstract class VideoBase<T>
    {
        internal VideoBase()
        {
            
        }
        public abstract HalogenId Id { get; set; }
        public abstract string Title { get; set; }
        public abstract TagSet Tags { get; set; }
        protected internal string Data {get;set;} = "";

        public virtual T GetData() {
            return System.Text.Json.JsonSerializer.Deserialize<T>(Data, new System.Text.Json.JsonSerializerOptions { AllowTrailingCommas = true});
        }

        public virtual void SaveData(T data) {
            Data = System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions { AllowTrailingCommas = true});
        }
    }
    public sealed class Video : VideoBase<List<string>>
    {
        /// <summary>
        /// This is not guaranteed to be set! Provides the name of the collection this video belongs to.
        /// If the video is in multiple collections, this will be the last collection it was added to.
        /// </summary>
        /// <value>The name of the current collection.</value>
        public string CollectionName { get; set; }

        public override HalogenId Id { get; set; }
        public override string Title { get; set; }
        public override TagSet Tags { get; set; }
    }

    public sealed class Video<T> : VideoBase<T>
    {
        public override HalogenId Id { get; set; }
        public override string Title { get; set; }
        public override TagSet Tags { get; set; }
    }
}