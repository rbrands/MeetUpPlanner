using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;

namespace MeetUpPlanner.Shared
{
    public class RouteFilter
    {
        public Boolean ForReview { get; set; }
        public Boolean OnlyForMembers { get; set; }
        public Boolean OnlyOwn { get; set; }
        public string Scope { get; set; }
        
        // The filter is build as a list of lists:
        // At least one tag of every inner list must be found. 
        public IList<IList<RouteTag>> Tags { get; set; }
        public RouteFilter()
        {
            Tags = new List<IList<RouteTag>>();
        }
    }
}
