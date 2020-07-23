using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace MeetUpPlanner.Shared
{
    public class ClientSettings : CosmosDBEntity
    {
        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; } = "MeetUp-Planner";
        [DataType(DataType.Url, ErrorMessage = "Bitte eine gültige URL eingeben.")]
        [JsonProperty(PropertyName = "furtherInfoLink", NullValueHandling = NullValueHandling.Ignore)]
        public string FurtherInfoLink { get; set; } = "https://scuderia-suedstadt.de";
        [JsonProperty(PropertyName = "furtherInfoTitle", NullValueHandling = NullValueHandling.Ignore)]
        public string FurtherInfoTitle { get; set; } = "Scuderia Südstadt";
        [JsonProperty(PropertyName = "logoLink", NullValueHandling = NullValueHandling.Ignore)]
        public string LogoLink { get; set; } 
        [JsonProperty(PropertyName = "welcomeMessage", NullValueHandling = NullValueHandling.Ignore)]
        public string WelcomeMessage { get; set; }
        [JsonProperty(PropertyName = "whiteboardMessage", NullValueHandling = NullValueHandling.Ignore)]
        public string WhiteboardMessage { get; set; }
        [JsonProperty(PropertyName = "newMeetupMessage", NullValueHandling = NullValueHandling.Ignore)]
        public string NewMeetupMessage { get; set; }
        [JsonProperty(PropertyName ="maxGroupSize", NullValueHandling = NullValueHandling.Ignore)]
        public int MaxGroupSize { get; set; } = 10;
    }
}
