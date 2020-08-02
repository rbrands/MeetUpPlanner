using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MeetUpPlanner.Shared;

namespace MeetUpPlanner.Functions
{
    public static class Extensions
    {
        /// <summary>
        /// Extension method to find a given person with given firstName and lastName. Case is ignored for comparison.
        /// </summary>
        /// <param name="participantList"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <returns></returns>
        public static Participant Find(this IEnumerable<Participant> participantList, string firstName, string lastName)
        {
            Participant participant = null;
            foreach (Participant p in participantList)
            {
                if (p.ParticipantFirstName.Equals(firstName, StringComparison.InvariantCultureIgnoreCase) && p.ParticipantLastName.Equals(lastName, StringComparison.InvariantCultureIgnoreCase))
                {
                    participant = p;
                    break;
                }
            }
            return participant;
        }
        public static void AddCompanion(this IList<Companion> companionList, string firstName, string lastName, string addressInfo, int sizeOfCalendarList, int calendarIndex) 
        {
            Companion companion = companionList.FirstOrDefault<Companion>(c => c.FirstName.Equals(firstName, StringComparison.InvariantCultureIgnoreCase)
                                                                            && c.LastName.Equals(lastName, StringComparison.InvariantCultureIgnoreCase));
            if (null == companion)
            {
                companion = new Companion(firstName, lastName, addressInfo, sizeOfCalendarList);
                companionList.Add(companion);
            }
            else
            {
                // Add addressInfo to get the latest info.
                companion.AddressInfo = addressInfo;
            }
            companion.EventList[calendarIndex] = true;
        }

        public static bool EqualsHost(this CalendarItem calendarItem, string firstName, string lastName)
        {
            return (calendarItem.HostFirstName.Equals(firstName, StringComparison.InvariantCultureIgnoreCase) && calendarItem.HostLastName.Equals(lastName, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
