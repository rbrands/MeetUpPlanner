using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MeetUpPlanner.Shared
{
    public class WinterpokalUser
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("link")]
        public string Link { get; set; }
        [JsonPropertyName("entries")]
        public long Entries { get; set; }
        [JsonPropertyName("points")]
        public long Points { get; set; }
        [JsonPropertyName("duration")]
        public long Duration { get; set; }
    }
}
