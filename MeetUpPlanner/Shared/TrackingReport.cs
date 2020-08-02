using System;
using System.Collections.Generic;
using System.Text;

namespace MeetUpPlanner.Shared
{
    /// <summary>
    /// Describes a "tracking report": A list of all persons who had participated with the user in an event in the past
    /// </summary>
    public class TrackingReport
    {
        /// <summary>
        /// Details about the requested report
        /// </summary>
        public TrackingReportRequest ReportRequest { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now; 
        /// <summary>
        /// List of all events in scope
        /// </summary>
        public IEnumerable<CompanionCalendarInfo> CalendarList { get; set; } 
        /// <summary>
        /// List of all companions in scope
        /// </summary>
        public IList<Companion> CompanionList { get; set; }

        public TrackingReport()
        {
        }
        /// <summary>
        /// Constructs a TrackingReport with the given request
        /// </summary>
        /// <param name="request"></param>
        public TrackingReport(TrackingReportRequest request)
        {
            ReportRequest = request;
        }
    }
}
