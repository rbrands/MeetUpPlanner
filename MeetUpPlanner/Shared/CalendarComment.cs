using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;


namespace MeetUpPlanner.Shared
{
    public class CalendarComment : CosmosDBEntity
    {
        /// <summary>
        /// Id of referenced CalendarItem
        /// </summary>
        [JsonProperty(PropertyName ="calendarItemId")]
        public string CalendarItemId { get; set; }
        [JsonProperty(PropertyName = "authorFirstName"), MaxLength(100), Required(ErrorMessage = "Vornamen bitte eingeben.")]
        public string AuthorFirstName { get; set; }
        [JsonProperty(PropertyName = "authorLastName"), MaxLength(100), Required(ErrorMessage = "Nachnamen bitte eingeben.")]
        public string AuthorLastName { get; set; }
        [JsonProperty(PropertyName = "commentDate")]
        public DateTime CommentDate { get; set; }
        [JsonProperty(PropertyName = "comment"), MaxLength(200, ErrorMessage = "Kommentar zu lang.")]
        public string Comment { get; set; }
        [JsonIgnore]
        public string AuthorDisplayName
        {
            get
            {
                if (null == AuthorFirstName) AuthorFirstName = String.Empty;
                if (null == AuthorLastName) AuthorLastName = " ";
                return AuthorFirstName + " " + AuthorLastName[0] + ".";
            }
        }
        public string DisplayDate
        {
            get
            {
                return (null != CommentDate) ? CommentDate.ToString("dd.MM. HH:mm") : String.Empty;
            }
        }

    }
}
