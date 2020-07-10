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
    }
}
