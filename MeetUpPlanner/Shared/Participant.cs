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
        [JsonProperty(PropertyName = "participantFirstName"), MaxLength(100)]
        public string ParticipantFirstName { get; set; }
        [JsonProperty(PropertyName = "participantLastName"), MaxLength(100)]
        public string ParticipantLastName { get; set; }
        [JsonProperty(PropertyName = "participantAdressInfo"), MaxLength(100)]
        public string ParticipantAdressInfo { get; set; }
        [JsonProperty(PropertyName = "checkInDate")]
        public DateTime CheckInDate { get; set; }
        [JsonProperty(PropertyName = "isGuest")]
        public Boolean IsGuest { get; set; } = false;
        [JsonProperty(PropertyName = "isWaiting")]
        public Boolean IsWaiting { get; set; } = false;
        [JsonProperty(PropertyName = "isCoGuide")]
        public Boolean IsCoGuide { get; set; } = false;
        [JsonProperty(PropertyName = "federaton")]
        public string Federation { get; set; }

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
        public string ParticipantDisplayNameWithCoGuideSuffix(int nameDisplayLength)
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
                if (IsCoGuide)
                {
                    sb.Append(" (Co-Guide)");
                }
            }

            return sb.ToString();
        }

    }
}
