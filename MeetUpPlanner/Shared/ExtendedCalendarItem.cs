using System;
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
        [JsonIgnore]
        public bool HideOlderCommments { get; set; } = true;

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
            this.RouteLink = calendarItem.RouteLink;
            this.HostFirstName = calendarItem.HostFirstName;
            this.HostLastName = calendarItem.HostLastName;
            this.HostAdressInfo = calendarItem.HostAdressInfo;
            this.Summary = calendarItem.Summary;
            this.MaxRegistrationsCount = calendarItem.MaxRegistrationsCount;
            this.MinRegistrationsCount = calendarItem.MinRegistrationsCount;
            this.MaxWaitingList = calendarItem.MaxWaitingList;
            this.MaxCoGuidesCount = calendarItem.MaxCoGuidesCount;
            this.PrivateKeyword = calendarItem.PrivateKeyword;
            this.IsInternal = calendarItem.IsInternal;
            this.LevelDescription = calendarItem.LevelDescription;
            this.Tempo = calendarItem.Tempo;
            this.Link = calendarItem.Link;
            this.LinkImage = calendarItem.LinkImage;
            this.LinkTitle = calendarItem.LinkTitle;
            this.ParticipantsList = new List<Participant>();
            this.CommentsList = new List<CalendarComment>();
            this.IsCross = calendarItem.IsCross;
            this.IsTraining = calendarItem.IsTraining;
            this.IsCanceled = calendarItem.IsCanceled;
            this.Tenant = calendarItem.Tenant;
            this.WithoutHost = calendarItem.WithoutHost;
            this.GuestScope = calendarItem.GuestScope;
            this.AttachedInfoType = calendarItem.AttachedInfoType;
            this.AttachedInfoKey = calendarItem.AttachedInfoKey;
            this.Federation = calendarItem.Federation;
            this.FederatedFrom = calendarItem.FederatedFrom;
            this.MaxCoGuidesCount = calendarItem.MaxCoGuidesCount;

        }

        public string ParticipantsDisplay(int nameDisplayLength, string firstName, string lastName)
        {
            StringBuilder sb = new StringBuilder(100);
            int counter = WithoutHost ? 0 : 1;
            int coGuideCounter = 0;
            bool isCheckedInAsIncognito = IsCheckedInAsIncognito(firstName, lastName);
            foreach (Participant participant in this.ParticipantsList)
            {
                if (participant.IsCoGuide) coGuideCounter++;
                Boolean isOwnName = participant.ParticipantFirstName.Equals(firstName) && participant.ParticipantLastName.Equals(lastName);
                if (!participant.IsWaiting)
                { 
                    if (counter > 0)
                    { 
                        sb.Append(", ");
                    }
                    if (participant.IsCoGuide && coGuideCounter <= this.MaxCoGuidesCount)
                    {
                        sb.Append("<i>");
                    }
                    sb.Append((isCheckedInAsIncognito && !isOwnName) ? "Inkognito" : participant.ParticipantDisplayName(nameDisplayLength));
                    if (participant.IsCoGuide && coGuideCounter <= this.MaxCoGuidesCount)
                    {
                        sb.Append("(Co-Guide)</i>");
                    }
                    ++counter;
                }
            }
            return sb.ToString();
        }
        public string WaitingListDisplay(int nameDisplayLength, string firstName, string lastName)
        {
            StringBuilder sb = new StringBuilder(100);
            int counter = 0;
            bool isCheckedInAsIncognito = IsCheckedInAsIncognito(firstName, lastName);
            foreach (Participant participant in this.ParticipantsList)
            {
                Boolean isOwnName = participant.ParticipantFirstName.Equals(firstName) && participant.ParticipantLastName.Equals(lastName);
                if (participant.IsWaiting)
                { 
                    if (counter > 0)
                    {
                        sb.Append(", ");
                    }
                    sb.Append((isCheckedInAsIncognito && !isOwnName) ? "Inkognito" : participant.ParticipantDisplayName(nameDisplayLength));
                    ++counter;
                }
            }
            return sb.ToString();
        }
        public IEnumerable<CalendarComment> GetMostRecentComments(int count)
        {
            List<CalendarComment> comments = new List<CalendarComment>(count);
            int i = 0;
            foreach (CalendarComment c in this.CommentsList)
            {
                comments.Add(c);
                ++i;
                if (i >= count) break;
            }
            return comments;
        }
        public IEnumerable<CalendarComment> GetOlderComments(int count)
        {
            List<CalendarComment> comments = new List<CalendarComment>(count);
            int i = 0;
            foreach (CalendarComment c in this.CommentsList)
            {
                ++i;
                if (i <= count) continue;
                comments.Add(c);
            }
            return comments;
        }
        [JsonIgnore]
        public int CommentsCounter
        {
            get
            {
                int counter = 0;
                foreach (CalendarComment c in this.CommentsList)
                {
                    ++counter;
                }
                return counter;
            }
        }
        [JsonIgnore]
        public int ParticipantCounter
        {
            get
            {
                int counter = WithoutHost ? 0 : 1;
                foreach (Participant p in ParticipantsList)
                {
                    if (!p.IsWaiting)
                    { 
                        ++counter;
                    }
                }
                return counter;
            }
        }
        [JsonIgnore]
        public int WaitingListCounter
        {
            get
            {
                int counter = 0;
                foreach (Participant p in ParticipantsList)
                {
                    if (p.IsWaiting)
                    { 
                        ++counter;
                    }
                }
                return counter;
            }
        }
        [JsonIgnore]
        public int CoGuidesCounter
        {
            get
            {
                int counter = 0;
                foreach (Participant p in ParticipantsList)
                {
                    if (p.IsCoGuide)
                    {
                        ++counter;
                    }
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
        public Boolean IsCheckedInAsIncognito(string firstName, string lastName)
        {
            Participant participant = FindParticipant(firstName, lastName);
            Boolean result = (null != participant && participant.IsIncognito);
            return result;
        }
    }
}

