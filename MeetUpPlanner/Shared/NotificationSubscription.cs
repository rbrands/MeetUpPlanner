using System;
using System.Collections.Generic;
using System.Text;

namespace MeetUpPlanner.Shared
{
    public class NotificationSubscription : CosmosDBEntity
    {
        public string PlannerUrl { get; set; }
        public string UserFirstName { get; set; }
        public string UserLastName { get; set; }
        public string Url { get; set; }
        public string P256dh { get; set; }
        public string Auth { get; set; }
    }
}
