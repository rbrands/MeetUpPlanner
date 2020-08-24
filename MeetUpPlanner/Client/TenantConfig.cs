using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetUpPlanner.Client
{
    /// <summary>
    /// Helper class to hold the configuration for tenants. 
    /// </summary>
    public static class TenantConfig
    {
        private static List<TenantSettings> _tenantList = new List<TenantSettings>
        {
            //new TenantSettings("ausfahrten.scuderia-suedstadt.de", null, true), // default is null (no special tenant)
            new TenantSettings("ausfahrten.robert-brands", "demo", false),
            new TenantSettings("localhost", "demo", true)
        };
        /// <summary>
        /// For the given URL all configured tenants are checked.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static TenantSettings GetTenant(string url)
        {
            TenantSettings tenant = new TenantSettings();
            foreach (TenantSettings t in _tenantList)
            {
                if (url.Contains(t.LookFor))
                {
                    tenant = t;
                    break;
                }
            }
            return tenant;
        }
    }

    public class TenantSettings
    {
        public string LookFor { get; set; } = null;
        public string Name { get; set; } = null;
        public bool ClubMembershipAllowed { get; set; }

        public TenantSettings()
        {
            ClubMembershipAllowed = true;
        }
        public TenantSettings(string lookFor, string name, bool clubMembershipAllowed)
        {
            LookFor = lookFor;
            Name = name;
            ClubMembershipAllowed = clubMembershipAllowed;
        }
    }
}
