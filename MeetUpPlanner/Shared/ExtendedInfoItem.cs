using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MeetUpPlanner.Shared
{
    /// <summary>
    /// InfoItem with attached list of comments for reading and tranfer to client.
    /// </summary>
    public class ExtendedInfoItem : InfoItem
    {
        [JsonProperty(PropertyName = "commentsList")]
        public IEnumerable<CalendarComment> CommentsList { get; set; } = new List<CalendarComment>();

        public ExtendedInfoItem()
        {

        }
        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="infoItem"></param>
        public ExtendedInfoItem(InfoItem infoItem)
        {
            this.AuthorFirstName = infoItem.AuthorFirstName;
            this.AuthorLastName = infoItem.AuthorLastName;
            this.CommentsAllowed = infoItem.CommentsAllowed;
            this.CommentsLifeTimeInDays = infoItem.CommentsLifeTimeInDays;
            this.HeaderTitle = infoItem.HeaderTitle;
            this.Id = infoItem.Id;
            this.InfoContent = infoItem.InfoContent;
            this.InfoLifeTimeInDays = infoItem.InfoLifeTimeInDays;
            this.IsHtmlCode = infoItem.IsHtmlCode;
            this.Link = infoItem.Link;
            this.LinkTitle = infoItem.LinkTitle;
            this.OrderId = infoItem.OrderId;
            this.Tenant = infoItem.Tenant;
            this.Title = infoItem.Title;
        }
    }
}
