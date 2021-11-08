using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MeetUpPlanner.Shared
{
    public class Route : CosmosDBEntity
    {
        [JsonProperty(PropertyName = "date"), Display(Name = "Datum"), UIHint("Date"), Required]
        public DateTime Date { get; set; } = DateTime.Today;
        [JsonProperty(PropertyName = "authorId"), Display(Name = "Autor")]
        public string AuthorId { get; set; }
        [JsonProperty(PropertyName = "reviewDate"), Display(Name = "Datum"), UIHint("Date"), Required]
        public DateTime ReviewDate { get; set; } = DateTime.Today;
        [JsonProperty(PropertyName = "reviewer"), Display(Name = "Reviewer")]
        public string ReviewerId { get; set; }
        [JsonProperty(PropertyName = "title", NullValueHandling = NullValueHandling.Ignore), MaxLength(120, ErrorMessage = "Titel zu lang."), Required(ErrorMessage = "Bitte Titel eingeben.")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "place", NullValueHandling = NullValueHandling.Ignore), Display(Name = "Ort", Prompt = "Wo findet die Veranstaltung statt bzw. wo ist der Start"), MaxLength(256)]
        public string Place { get; set; }
        [JsonProperty(PropertyName = "DirectionsLink"), Display(Name = "Link zum Startpunkt", Prompt = "Link zum Startpunkt der Ausfahrt."), MaxLength(256), UIHint("url")]
        public string DirectionsLink { get; set; }
        [JsonProperty(PropertyName = "scope", NullValueHandling = NullValueHandling.Ignore), Display(Name = "Titel-Link", Prompt = "Kurztitel wie er in der Url auftaucht"), MaxLength(160, ErrorMessage = "UrlTitel zu lang.")]
        [RegularExpression("[a-z0-9-_]*", ErrorMessage = "Bitte nur Kleinbuchstaben und Zahlen für den Titel-Link eingeben.")]
        public string Scope { get; set; }
        [JsonProperty(PropertyName = "levelDescription"), Required(ErrorMessage = "Bitte Angaben zur Länge/Dauer machen."), MaxLength(60, ErrorMessage = "Angabe zur Länge bitte kürzen.")]
        public string LevelDescription { get; set; }
        [JsonProperty(PropertyName = "description"), Display(Name = "Beschreibung"), MaxLength(5000, ErrorMessage = "Die Beschreibung ist zu lang.")]
        public string Description { get; set; }
        [JsonProperty(PropertyName = "photosLink", NullValueHandling = NullValueHandling.Ignore), Display(Name = "Link zu Fotos"), UIHint("Url")]
        public string PhotosLink { get; set; }
        [JsonProperty(PropertyName = "videoLink", NullValueHandling = NullValueHandling.Ignore), Display(Name = "Link zu einem Video", Prompt = "Link zu einem Video"), UIHint("Url")]
        public string VideoLink { get; set; }
        [JsonProperty(PropertyName = "routeLink", NullValueHandling = NullValueHandling.Ignore), Display(Name = "Route", Prompt = "Link zur Strecke"), UIHint("Url")]
        public string RouteLink { get; set; }
        [JsonProperty(PropertyName = "routeLinkImage", NullValueHandling = NullValueHandling.Ignore), Display(Name = "Image", Prompt = "Link zu einem Image"), UIHint("Url")]
        public string RouteLinkImage { get; set; }
        [JsonProperty(PropertyName = "routeLinkTitle", NullValueHandling = NullValueHandling.Ignore), Display(Name = "Link-Bezeichnung"), MaxLength(120, ErrorMessage = "Link-Bezeichnung zu lang.")]
        public string RouteLinkTitle { get; set; }
        [JsonProperty(PropertyName = "gpxLink", NullValueHandling = NullValueHandling.Ignore), Display(Name = "GPX", Prompt = "Link zu GPX Datei"), UIHint("Url")]
        public string GpxLink { get; set; }
        [JsonProperty(PropertyName = "stravaLink", NullValueHandling = NullValueHandling.Ignore), Display(Name = "Strava", Prompt = "Link zu Strava"), UIHint("Url")]
        public string StravaLink { get; set; }
        [JsonProperty(PropertyName = "isReviewed")]
        public Boolean IsReviewed { get; set; }
        [JsonProperty(PropertyName = "isNonPublic")]
        public Boolean IsNonPublic { get; set; }
        [JsonProperty(PropertyName = "tags"), Display(Name = "Tags"), MaxLength(512, ErrorMessage = "Zu viele Tags.")]
        public string Tags { get; set; }
        public IList<RouteTag> RouteTags { get; set; } = new List<RouteTag>();

        public string GetUrlFriendlyTitle()
        {
            string urlFriendlyTitle = null;
            if (!String.IsNullOrEmpty(Title))
            {
                string titleLowerCase = Title.ToLowerInvariant();
                StringBuilder sb = new StringBuilder();
                int charCounter = 0;
                foreach(char c in titleLowerCase)
                {
                    if (++charCounter > 160)
                    {
                        // url not longer than 160 chars
                        break;
                    }
                    switch (c)
                    {
                        case 'ö':
                        case 'Ö':
                            sb.Append("oe");
                            break;
                        case 'ü':
                        case 'Ü':
                            sb.Append("ue");
                            break;
                        case 'ä':
                        case 'Ä':
                            sb.Append("ae");
                            break;
                        case 'ß':
                            sb.Append("ss");
                            break;
                        case 'a':
                        case 'b':
                        case 'c':
                        case 'd':
                        case 'e':
                        case 'f':
                        case 'g':
                        case 'h':
                        case 'i':
                        case 'j':
                        case 'k':
                        case 'l':
                        case 'm':
                        case 'n':
                        case 'o':
                        case 'p':
                        case 'q':
                        case 'r':
                        case 's':
                        case 't':
                        case 'u':
                        case 'v':
                        case 'w':
                        case 'x':
                        case 'y':
                        case 'z':
                        case '0':
                        case '1':
                        case '2':
                        case '3':
                        case '4':
                        case '5':
                        case '6':
                        case '7':
                        case '8':
                        case '9':
                            sb.Append(c);
                            break;
                        default:
                            sb.Append('-');
                            break;
                    }
                }
                urlFriendlyTitle = sb.ToString();
            }
            return urlFriendlyTitle;
        }
    }

    public class RouteTag
    {
        public string TagSetId { get; set; }
        public string TagId { get; set; }
        public string TagLabel { get; set; }
        public TagBadgeColor BadgeColor { get; set; }
    }
}
