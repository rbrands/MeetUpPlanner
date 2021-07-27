using System;
using System.Collections.Generic;
using System.Text;

namespace MeetUpPlanner.Shared
{
    /// <summary>
    /// Returns the result of keyword check
    /// </summary>
    public class KeywordCheck
    {
        /// <summary>
        /// Exploring and joining MeetUps without keyword
        /// </summary>
        public bool IsParticipant { get; set; }
        /// <summary>
        /// Is true if the given keyword matches against the public or admin keyword
        /// </summary>
        public bool IsUser { get; set; }
        /// <summary>
        /// Is true if the given keyword matches against the admin keyword
        /// </summary>
        public bool IsAdmin { get; set; }
    }
}
