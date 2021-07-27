using System;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MeetUpPlanner.Shared
{
    public class LinkPreview : CosmosDBEntity
    {
        public Boolean Success { get; set; }
        public string Message { get; set; } 
        public string Title { get; set; }
        public string Description { get; set; }
        public Uri ImageUrl { get; set; }
        public Uri Url { get; set; }
        public Uri CanoncialUrl { get; set; }
    }
}
