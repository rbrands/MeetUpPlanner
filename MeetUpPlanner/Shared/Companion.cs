using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;


namespace MeetUpPlanner.Shared
{
    /// <summary>
    /// Class to get contact details of a companion during an event in the past
    /// </summary>
    public class Companion
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string AdressInfo { get; set; }
        /// <summary>
        /// List of CalendarItem the user was part of
        /// </summary>
        IEnumerable<CompanionCalendarInfo> EventList { get; set; }
    }
}
