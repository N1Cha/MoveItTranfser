using Newtonsoft.Json;

namespace Infrastructure.Models
{
    public class File
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("path")]
        public string Path { get; set; }
    }
}
