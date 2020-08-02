using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
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
        public string AddressInfo { get; set; }
        /// <summary>
        /// List of true/false indicators showing if the user was part of the corresponding item in the list of CompanionCalendarInfo items
        /// in the TrackingReport
        /// </summary>
        public IList<bool> EventList { get; set; }
        public Companion()
        {

        }
        public Companion(string firstName, string lastName, string addressInfo, int eventListSize)
        {
            FirstName = firstName;
            LastName = lastName;
            AddressInfo = addressInfo;
            EventList = new List<bool>(eventListSize);
            for (int i = 0; i < eventListSize; ++ i)
            {
                EventList.Add(false);
            }
        }
    }
}
