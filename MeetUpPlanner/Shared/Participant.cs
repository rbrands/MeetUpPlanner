using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;


namespace MeetUpPlanner.Shared
{
    public class Participant : CosmosDBEntity 
    {
        /// <summary>
        /// Referenced CalendarItem
        /// </summary>
        [JsonProperty(PropertyName = "calendarItemId")]
        public string CalendarItemId { get; set; }
        [JsonProperty(PropertyName = "participantFirstName"), MaxLength(100), Required(ErrorMessage = "Vornamen bitte eingeben.")]
        public string ParticipantFirstName { get; set; }
        [JsonProperty(PropertyName = "participantLastName"), MaxLength(100), Required(ErrorMessage = "Nachnamen bitte eingeben.")]
        public string ParticipantLastName { get; set; }
        [JsonProperty(PropertyName = "checkInDate")]
        public DateTime CheckInDate { get; set; }
        [JsonIgnore]
        public string ParticipantDisplayName
        {
            get
            {
                return ParticipantFirstName + " " + ParticipantLastName[0] + ".";
            }
        }

    }
}
