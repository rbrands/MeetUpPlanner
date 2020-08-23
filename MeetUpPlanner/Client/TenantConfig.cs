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
        private static List<Tuple<string, string>> _tenantList = new List<Tuple<string, string>>
        {
            new Tuple<string, string>("ausfahrten.scuderia-suedstadt.de", null), // default is null (no special tenant)
            new Tuple<string, string>("ausfahrten.robert-brands", "demo"),
            new Tuple<string, string>("localhost", "demo")
        };
        /// <summary>
        /// For the given URL all configured tenants are checked.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string GetTenant(string url)
        {
            string tenant = null;
            foreach (Tuple<string, string> t in _tenantList)
            {
                if (url.Contains(t.Item1))
                {
                    tenant = t.Item2;
                    break;
                }
            }
            return tenant;
        }
    }
}
