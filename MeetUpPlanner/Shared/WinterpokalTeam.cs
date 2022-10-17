using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MeetUpPlanner.Shared
{
    public class WinterpokalTeam
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("link")]
        public string Link { get; set; }
        [JsonPropertyName("leader")]
        public WinterpokalUser Leader { get; set; }
        [JsonPropertyName("users")]
        public IList<WinterpokalUser> Users { get; set; } = new List<WinterpokalUser>();
        public long TeamPoints {
            get
            {
                long sum = 0;
                foreach (WinterpokalUser user in Users)
                {
                    sum += user.Points;
                }
                return sum;
            }
        }
    }
}
