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
        [JsonProperty(PropertyName = "helpLink", NullValueHandling = NullValueHandling.Ignore)]
        public string HelpLink { get; set; } = "https://www.meetupplanner.de/userguide";
        [JsonProperty(PropertyName = "welcomeMessage", NullValueHandling = NullValueHandling.Ignore)]
        public string WelcomeMessage { get; set; }
        [JsonProperty(PropertyName = "whiteboardMessage", NullValueHandling = NullValueHandling.Ignore)]
        public string WhiteboardMessage { get; set; }
        [JsonProperty(PropertyName = "newMeetupMessage", NullValueHandling = NullValueHandling.Ignore)]
        public string NewMeetupMessage { get; set; }
        [JsonProperty(PropertyName ="maxGroupSize", NullValueHandling = NullValueHandling.Ignore), Range(2.0, 300, ErrorMessage = "Max. Gruppengröße bitte zwischen 2 und 300.")]
        public int MaxGroupSize { get; set; } = 10;
        [JsonProperty(PropertyName = "maxGroupSizeForAdmins", NullValueHandling = NullValueHandling.Ignore), Range(2.0, 300, ErrorMessage = "Max. Gruppengröße für Admins bitte zwischen 2 und 300.")]
        public int MaxGroupSizeForAdmins { get; set; } = 10;
        [JsonProperty(PropertyName = "nameDisplayLength"), Range(0.0, 10, ErrorMessage = "Länge des Anzeigenamens zwischen 0 und 10.")]
        public int NameDisplayLength { get; set; } = 1;
        public Boolean GuestsEnabled { get; set; } = false;
        public Boolean OnlyScopedMeetUpsAllowed { get; set; } = false;
        public Boolean OnlyAdminsAsHost { get; set; } = false;
        public Boolean AppLocked { get; set; } = false;
        [JsonProperty(PropertyName = "lockedMessage", NullValueHandling = NullValueHandling.Ignore)]
        public string LockedMessage { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore), MaxLength(30, ErrorMessage = "Wochenkennzeichnung bitte kürzer als 30 Zeichen.")]
        public string MondayBadge { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore), MaxLength(30, ErrorMessage = "Wochenkennzeichnung bitte kürzer als 30 Zeichen.")]
        public string TuesdayBadge { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore), MaxLength(30, ErrorMessage = "Wochenkennzeichnung bitte kürzer als 30 Zeichen.")]
        public string WednesdayBadge { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore), MaxLength(30, ErrorMessage = "Wochenkennzeichnung bitte kürzer als 30 Zeichen.")]
        public string ThursdayBadge { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore), MaxLength(30, ErrorMessage = "Wochenkennzeichnung bitte kürzer als 30 Zeichen.")]
        public string FridayBadge { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore), MaxLength(30, ErrorMessage = "Wochenkennzeichnung bitte kürzer als 30 Zeichen.")]
        public string SaturdayBadge { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore), MaxLength(30, ErrorMessage = "Wochenkennzeichnung bitte kürzer als 30 Zeichen.")]
        public string SundayBadge { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Disclaimer { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string GuestDisclaimer { get; set; }

        public int GetMaxGroupSize(bool isAdmin)
        {
            int maxGroupSize = MaxGroupSize;
            if (isAdmin && MaxGroupSizeForAdmins > maxGroupSize)
            {
                maxGroupSize = MaxGroupSizeForAdmins;
            }
            return maxGroupSize;
        }
    }
}
