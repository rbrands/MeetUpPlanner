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
        [JsonProperty(PropertyName = "participantAddressName", NullValueHandling = NullValueHandling.Ignore), MaxLength(100), Required(ErrorMessage = "Bitte eine Adress-Info eingeben.")]
        public string ParticipantAdressInfo { get; set; }
        [JsonProperty(PropertyName = "checkInDate")]
        public DateTime CheckInDate { get; set; }
        [JsonProperty(PropertyName = "isGuest")]
        public Boolean IsGuest { get; set; } = false;

        public Participant()
        {

        }
        public Participant(string firstName, string lastName, string adressInfo)
        {
            ParticipantFirstName = firstName;
            ParticipantLastName = lastName;
            ParticipantAdressInfo = adressInfo;
        }
        public string ParticipantDisplayName(int nameDisplayLength)
        {
            StringBuilder sb = new StringBuilder();
            if (IsGuest)
            {
                sb.Append("Gast");
            }
            else
            { 
                int length = nameDisplayLength > 0 ? Math.Min(nameDisplayLength, ParticipantLastName.Length) : ParticipantLastName.Length;
                sb.Append(ParticipantFirstName).Append(" ");
                sb.Append(ParticipantLastName.Substring(0, length));
                if (length < ParticipantLastName.Length)
                {
                    sb.Append('.');
                }
            }

            return sb.ToString();
        }

    }
}
