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
        [JsonProperty(PropertyName = "participantAddressName", NullValueHandling = NullValueHandling.Ignore), MaxLength(100) ]
        public string ParticipantAdressInfo { get; set; }
        [JsonProperty(PropertyName = "checkInDate")]
        public DateTime CheckInDate { get; set; }
        [JsonProperty(PropertyName = "isGuest")]
        public Boolean IsGuest { get; set; } = false;
        [JsonIgnore]
        public string ParticipantDisplayName
        {
            get
            {
                return (IsGuest ? "Gast" : ParticipantFirstName + " " + ParticipantLastName[0] + ".");
            }
        }

    }
}
