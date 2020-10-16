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
            //                              lookfor                  tenant ClubMemberShipAllowed GuestNameShown 
            //new TenantSettings("ausfahrten.scuderia-suedstadt.de", null,         true,                 false), // default is null (no special tenant)
            new TenantSettings("ausfahrten.robert-brands",           "demo",       false,                true),
            new TenantSettings("demo.meetupplanner",                 "demo",       false,                true),
            new TenantSettings("ausfahrten.dasimmerdabei",           "dsd",        true,                 true),
            new TenantSettings("fbgiro.meetupplanner",               "fbgiro",     false,                true),
            new TenantSettings("ausfahrten.flannelspandex",          "fs",         false,                true),
            new TenantSettings("meetup-planner-dev.azurewebsites.net","demo",      false,                true),
            new TenantSettings("localhost",                          "fs",         false,                true)
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
        public bool GuestNameShown { get; set; }

        public TenantSettings()
        {
            ClubMembershipAllowed = true;
            GuestNameShown = false;
        }
        public TenantSettings(string lookFor, string name, bool clubMembershipAllowed, bool guestNameShown)
        {
            LookFor = lookFor;
            Name = name;
            ClubMembershipAllowed = clubMembershipAllowed;
            GuestNameShown = guestNameShown;
        }
    }
}
