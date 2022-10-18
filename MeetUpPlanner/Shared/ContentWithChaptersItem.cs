using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;


namespace MeetUpPlanner.Shared
{
	public class ContentWithChaptersItem : CosmosDBEntity
	{
		public class Chapter
		{
			[JsonProperty(PropertyName = "title")]
			public string Title { get; set; }
            [JsonProperty(PropertyName = "name")]
            public string Name { get; set; }
            [JsonProperty(PropertyName = "content")]
            public string Content { get; set; }
		}

        [JsonProperty(PropertyName = "title")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "chapters")]
        IList<Chapter> Chapters { get; set; } = new List<Chapter>();
        [JsonProperty(PropertyName = "infoLifeTimeInDays", NullValueHandling = NullValueHandling.Ignore), Range(0.0, 100.0, ErrorMessage = "Lebensdauer der Info nicht im gültigen Bereich."), Display(Name = "Lebensdauer der Info", Prompt = "Wie viel Tage soll die Info gespeichert werden? (0 für keine automatische Löschung."), Required(ErrorMessage = "Lebensdauer für die Info eingeben.")]
        public int InfoLifeTimeInDays { get; set; } = 0;

    }
}
