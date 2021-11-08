using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MeetUpPlanner.Shared
{
    public class TagSet : CosmosDBEntity
    {
        [JsonProperty(PropertyName = "name"), Display(Name = "Name"), Required(ErrorMessage = "Bitte Namen angeben."), MaxLength(120, ErrorMessage = "Name zu lang.")]
        public string Name { get; set; }
        [JsonProperty(PropertyName = "isMandatory")]
        public Boolean IsMandatory { get; set; } = false;
        [JsonProperty(PropertyName = "isMutuallyExclusive")]
        public Boolean IsMutuallyExclusive { get; set; } = false;
        [JsonProperty(PropertyName = "hasRestrictedAccess")]
        public Boolean HasRestrictedAccess { get; set; } = false;
        [JsonProperty(PropertyName = "badgeColor")]
        public TagBadgeColor BadgeColor { get; set; }
        [JsonProperty(PropertyName = "tags")]
        public IList<Tag> Tags { get; set; } = new List<Tag>();
        public void AddTag(Tag tag)
        {
            if (!Tags.Contains(tag))
            {
                Tags.Add(tag);
            }
        }

        public string GetAllTagsAsString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Tag tag in Tags)
            {
                sb.Append(tag.Label + " ");
            }
            return sb.ToString();
        }
     }

    public class Tag
    {
        [JsonProperty(PropertyName = "tagId"), Display(Name = "Tag Id")]
        public string TagId { get; set; } = System.Guid.NewGuid().ToString();
        [JsonProperty(PropertyName = "label"), Display(Name = "Tag Label"), MaxLength(30, ErrorMessage = "Label bitte nicht länger als 30 Zeichen")]
        [RegularExpression("[a-zA-Z0-9-<>üÜöÖäÄß]*", ErrorMessage = "Bitte nur Buchstaben und Zahlen für das Label.")]
        public string Label { get; set; }
        [JsonIgnore]
        public Boolean HideFromSelection;
    }

    public enum TagBadgeColor
    {
        Primary = 0,
        Secondary,
        Success,
        Danger,
        Warning,
        Info,
        Light,
        Dark
    }
}
