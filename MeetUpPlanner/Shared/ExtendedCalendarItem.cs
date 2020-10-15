using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MeetUpPlanner.Shared
{
    /// <summary>
    /// CalendarItem with attached list of particpants and comments for reading and tranfer to client.
    /// </summary>
    public class ExtendedCalendarItem : CalendarItem
    {
        [JsonProperty(PropertyName = "participantsList")]
        public IEnumerable<Participant> ParticipantsList { get; set; } = new List<Participant>();
        [JsonProperty(PropertyName = "commentsList")]
        public IEnumerable<CalendarComment> CommentsList { get; set; } = new List<CalendarComment>();

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
            this.PublishDate = calendarItem.PublishDate;
            this.Weekly = calendarItem.Weekly;
            this.IsCopiedToNextWeek = calendarItem.IsCopiedToNextWeek;
            this.Place = calendarItem.Place;
            this.DirectionsLink = calendarItem.DirectionsLink;
            this.HostFirstName = calendarItem.HostFirstName;
            this.HostLastName = calendarItem.HostLastName;
            this.HostAdressInfo = calendarItem.HostAdressInfo;
            this.Summary = calendarItem.Summary;
            this.MaxRegistrationsCount = calendarItem.MaxRegistrationsCount;
            this.MinRegistrationsCount = calendarItem.MinRegistrationsCount;
            this.PrivateKeyword = calendarItem.PrivateKeyword;
            this.LevelDescription = calendarItem.LevelDescription;
            this.Tempo = calendarItem.Tempo;
            this.Link = calendarItem.Link;
            this.LinkTitle = calendarItem.LinkTitle;
            this.ParticipantsList = new List<Participant>();
            this.CommentsList = new List<CalendarComment>();
            this.IsCross = calendarItem.IsCross;
            this.IsCanceled = calendarItem.IsCanceled;
            this.Tenant = calendarItem.Tenant;
            this.WithoutHost = calendarItem.WithoutHost;
            this.GuestScope = calendarItem.GuestScope;
        }

        public string ParticipantsDisplay(int nameDisplayLength)
        {
            StringBuilder sb = new StringBuilder(100);
            int counter = WithoutHost ? 0 : 1;
            foreach (Participant participant in this.ParticipantsList)
            {
                if (counter > 0)
                { 
                    sb.Append(", ");
                }
                sb.Append(participant.ParticipantDisplayName(nameDisplayLength));
                ++counter;
            }
            return sb.ToString();
        }
        [JsonIgnore]
        public int ParticipantCounter
        {
            get
            {
                int counter = WithoutHost ? 0 : 1;
                foreach (Participant p in ParticipantsList)
                {
                    ++counter;
                }
                return counter;
            }
        }
        /// <summary>
        /// Helper function to look for given person in the participants list
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns>Participant object if found otherwise null</returns>
        public Participant FindParticipant(string firstName, string lastName)
        {
            Participant participant = null;
            if (null != ParticipantsList)
            { 
                foreach (Participant p in ParticipantsList)
                {
                    if (p.ParticipantFirstName.Equals(firstName) && p.ParticipantLastName.Equals(lastName))
                    {
                        participant = p;
                        break;
                    }
                }
            }
            return participant;
        }


    }
}

