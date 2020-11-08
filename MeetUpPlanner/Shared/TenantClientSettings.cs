using System;
using System.Collections.Generic;
using System.Text;

namespace MeetUpPlanner.Shared
{
    public class TenantClientSettings
    {
        public TenantSettings Tenant { get; set; }
        public ClientSettings Client { get; set; }
    }
}
