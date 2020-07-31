using System;
using System.Collections.Generic;
using System.Text;

namespace MeetUpPlanner.Shared
{
    /// <summary>
    /// Class holds the info if a companion was part of the event
    /// </summary>
    public class CompanionCalendarInfo
    {
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public string HostFirstName { get; set; }
        public string HostLastName { get; set; }
        public string LevelDescription { get; set; }

        public CompanionCalendarInfo()
        {

        }
        public CompanionCalendarInfo(CalendarItem calendarItem)
        {
            Title = calendarItem.Title;
            StartDate = calendarItem.StartDate;
            HostFirstName = calendarItem.HostFirstName;
            HostLastName = calendarItem.HostLastName;
            LevelDescription = calendarItem.LevelDescription;
        }
    }
}
