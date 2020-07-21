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
        [Required(ErrorMessage = "Bitte ein Schlüsselwort für den Zugriff vergeben.")]
        public string UserKeyword { get; set; } = "Abstand";
        [JsonProperty(PropertyName = "adminKeyword")]
        [Required(ErrorMessage = "Bitte ein Schlüsselwort für den Admin-Zugriff vergeben.")]
        public string AdminKeyword { get; set; } = "AdminPassword";
        /// <summary>
        /// After given days meetups are deleted
        /// </summary>
        [JsonProperty(PropertyName = "autoDeleteAfterDays")]
        [Required(ErrorMessage = "Speicherzeitraum für die Termine in Tagen eingeben.")]
        [Range(1, 365, ErrorMessage = "Bitte als Speicherzeitraum einen Wert zwischen 1 und 365 Tagen eingeben.")]
        public int AutoDeleteAfterDays { get; set; } = 28;
        /// <summary>
        /// Checks if the given keyword matches the admin keyword
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public bool IsAdmin(string keyword)
        {
            return AdminKeyword.Equals(keyword);
        }
        /// <summary>
        /// Checks if the given keyword matches the user keyword or the admin keyword
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public bool IsUser(string keyword)
        {
            return this.UserKeyword.Equals(keyword) || this.AdminKeyword.Equals(keyword);
        }
    }
}
