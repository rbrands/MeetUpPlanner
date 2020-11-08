using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MeetUpPlanner.Shared;

namespace MeetUpPlanner.Functions
{
    public static class TenantConfig
    {
        private static List<TenantSettings> _tenantList = new List<TenantSettings>
        {
            //                              url                     tenant ClubMemberShipAllowed GuestNameShown 
            //new TenantSettings("https://ausfahrten.mydomain.de",   null,        true,             false), // default is null (no special tenant)
            //new TenantSettings("https://ausfahrten.robert-brands",  "demo",       false,            true),
        };
        /// <summary>
        /// For the given URL all configured tenants are checked.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static TenantSettings GetTenant(string url)
        {
            TenantSettings tenant = _tenantList.FirstOrDefault(t => url.StartsWith(t.PrimaryUrl));
            if (null == tenant)
            {
                tenant = new TenantSettings();
            }
            return tenant;
        }
    }
}
