using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MeetUpPlanner.Shared
{
    public class Comment : CosmosDBEntity
    {
        [JsonProperty(PropertyName = "referenceId")]
        public string ReferenceId { get; set; }
        [JsonProperty(PropertyName = "authorId"), Display(Name = "Autor")]
        public string AuthorId { get; set; }
        [JsonProperty(PropertyName = "authorDisplayName"), Display(Name = "Autor")]
        public string AuthorDisplayName { get; set; }
        [JsonProperty(PropertyName = "commentDate")]
        public DateTime CommentDate { get; set; } = DateTime.UtcNow;
        [JsonProperty(PropertyName = "commentText"), MaxLength(350, ErrorMessage = "Kommentar zu lang.")]
        public string CommentText { get; set; }
        [JsonProperty(PropertyName = "link", NullValueHandling = NullValueHandling.Ignore), MaxLength(200, ErrorMessage = "Link zu lang.")]
        public string Link { get; set; }
        [JsonProperty(PropertyName = "linkTitle", NullValueHandling = NullValueHandling.Ignore), MaxLength(80, ErrorMessage = "Link-Titel zu lang.")]
        public string LinkTitle { get; set; }
        [JsonIgnore]
        public string DisplayDate
        {
            get
            {
                return (null != CommentDate) ? CommentDate.ToLocalTime().ToString("dd.MM. HH:mm") : String.Empty;
            }
        }
        [JsonIgnore]

        public string DisplayLinkTitle
        {
            get
            {
                return String.IsNullOrEmpty(LinkTitle) ? "Link ..." : LinkTitle;
            }
        }

    }
}
