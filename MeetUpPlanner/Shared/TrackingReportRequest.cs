using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;


namespace MeetUpPlanner.Shared
{
    /// <summary>
    /// Request to get a tracking report
    /// </summary>
    public class TrackingReportRequest
    {
        public string RequestorFirstName { get; set; }
        public string RequestorLastName { get; set; }
        [Required(ErrorMessage = "Bitte Vornamen angeben.")]
        public string TrackFirstName { get; set; }
        [Required(ErrorMessage = "Bitte Nachnamen angeben.")]
        public string TrackLastName { get; set; }
        [Required(ErrorMessage = "Bitte einen Grund angeben.")]
        public string Comment { get; set; }
    }
}
