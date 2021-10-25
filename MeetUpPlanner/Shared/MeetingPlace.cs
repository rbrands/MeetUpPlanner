using System;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MeetUpPlanner.Shared
{
    public class MeetingPlace : CosmosDBEntity
    {
        [JsonProperty(PropertyName = "title", NullValueHandling = NullValueHandling.Ignore), Required(ErrorMessage = "Bitte Ortsbezeichnung eingeben."), Display(Name = "Titel", Prompt = "Titel der Info"), MaxLength(120, ErrorMessage = "Titel zu lang.")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "link", NullValueHandling = NullValueHandling.Ignore), MaxLength(250, ErrorMessage = "Link zu lang"), UIHint("url")]
        public string Link { get; set; }
    }
}
