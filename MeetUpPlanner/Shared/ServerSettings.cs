using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace MeetUpPlanner.Shared
{
    /// <summary>
    /// Settings used and needed at backend side of the planner
    /// </summary>
    public class ServerSettings : CosmosDBEntity
    {
        [JsonProperty(PropertyName = "userKeyword")]
        public string UserKeyword { get; set; }
        [JsonProperty(PropertyName = "adminKeyword")]
        public string AdminKeyword { get; set; }

    }
}
