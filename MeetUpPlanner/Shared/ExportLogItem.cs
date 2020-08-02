using System;
using System.Collections.Generic;
using System.Text;

namespace MeetUpPlanner.Shared
{
    public class ExportLogItem : CosmosDBEntity
    {
        public string RequestorFirstName { get; set; }
        public string RequestorLastName { get; set; }
        public string RequestedFirstName { get; set; }
        public string RequestedLastName { get; set; }
        public DateTime RequestDate { get; set; } = DateTime.Now;
        public string RequestReason { get; set; }

        public ExportLogItem()
        {

        }
        public ExportLogItem(TrackingReportRequest reportRequest)
        {
            RequestorFirstName = reportRequest.RequestorFirstName;
            RequestorLastName = reportRequest.RequestorLastName;
            RequestedFirstName = reportRequest.TrackFirstName;
            RequestedLastName = reportRequest.TrackLastName;
            RequestDate = DateTime.Now;
            RequestReason = reportRequest.Comment;

        }
    }
}
