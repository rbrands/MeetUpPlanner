using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MeetUpPlanner.Shared
{
    public class ExtendedRoute
    {
        public Route Core { get; set; }
        public UserContactInfo Author { get; set; }
        public string AuthorDisplayName { get; set; }
        public UserContactInfo Reviewer { get; set; }
        public string ReviewerDisplayName { get; set; }
        public DateTime LastUpdate {get;set;}
        public IEnumerable<ExtendedComment> CommentsList { get; set; } = new List<ExtendedComment>();


        public ExtendedRoute()
        {
            Core = new Route();
        }
        /// <summary>
        /// "Copy" constructor with instance of base class
        /// </summary>
        /// <param name="route"></param>
        public ExtendedRoute(Route route)
        {
            Core = route;
        }
        [JsonIgnore]
        public string DisplayLinkTitle
        {
            get
            {
                string title = this.Core.RouteLinkTitle;
                if (!String.IsNullOrEmpty(this.Core.RouteLink) && String.IsNullOrEmpty(this.Core.RouteLinkTitle))
                {
                    if (this.Core.RouteLink.Contains("komoot"))
                    {
                        title = "Tour auf Komoot";
                    }
                    else if (this.Core.RouteLink.Contains("strava"))
                    {
                        title = "Tour auf Strava";
                    }
                    else
                    {
                        title = "Weitere Info...";
                    }
                }
                return title;
            }
        }

    }
}
