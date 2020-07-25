using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.Data;

namespace MeetUpPlanner.Shared
{
    /// <summary>
    /// CalendarItem with attached list of particpants and comments for reading and tranfer to client.
    /// </summary>
    public class ExtendedCalendarItem : CalendarItem
    {
        [JsonProperty(PropertyName = "participantsList")]
        public IEnumerable<Participant> ParticipantsList { get; set; }
        [JsonProperty(PropertyName = "commentsList")]
        public IEnumerable<CalendarComment> CommentsList { get; set; }

        public ExtendedCalendarItem()
        {

        }
        /// <summary>
        /// "Copy" constructor with instance of base class CalendarItem
        /// </summary>
        /// <param name="calendarItem"></param>
        public ExtendedCalendarItem(CalendarItem calendarItem)
        {
            this.Id = calendarItem.Id;
            this.Title = calendarItem.Title;
            this.StartDate = calendarItem.StartDate;
            this.Place = calendarItem.Place;
            this.HostFirstName = calendarItem.HostFirstName;
            this.HostLastName = calendarItem.HostLastName;
            this.Summary = calendarItem.Summary;
            this.MaxRegistrationsCount = calendarItem.MaxRegistrationsCount;
            this.PrivateKeyword = calendarItem.PrivateKeyword;
            this.LevelDescription = calendarItem.LevelDescription;
            this.Tempo = calendarItem.Tempo;
            this.Link = calendarItem.Link;
        }

        [JsonIgnore]
        public string ParticipantsDisplay
        {
            get 
            {
                StringBuilder sb = new StringBuilder(100);
                foreach (Participant participant in this.ParticipantsList)
                {
                    sb.Append(", ");
                    sb.Append(participant.ParticipantDisplayName);
                }
                return sb.ToString();
            }
        }
        [JsonIgnore]
        public int ParticipantCounter
        {
            get
            {
                int counter = 1;
                foreach (Participant p in ParticipantsList)
                {
                    ++counter;
                }
                return counter;
            }
        }


    }
}

