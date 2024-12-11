using Newtonsoft.Json;

namespace Infrastructure.Models
{
    public class Folder
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("parentId")]
        public string ParentId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }

        [JsonProperty("lastContentChangeTime")]
        public DateTime LastContentChangeTime { get; set; }

        [JsonProperty("folderType")]
        public string FolderType { get; set; }
    }
}
