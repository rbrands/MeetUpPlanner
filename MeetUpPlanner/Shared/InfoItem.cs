using System;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MeetUpPlanner.Shared
{
    public class InfoItem : CosmosDBEntity
    {
        [JsonProperty(PropertyName = "orderId"), Range(0.0, 1000.0, ErrorMessage = "Ordnungszahl zur Steuerung der Reihenfolge nicht im gültigen Bereich."), Display(Name = "Ordnungszahl zur Steuerung der Reihenfolge.", Prompt = "Ordnungszahl zur Steuerung der Reihenfolge"), Required(ErrorMessage = "Ordnungszahl zur Steuerung der Reihenfolge eingeben.")]
        public int OrderId { get; set; } = 100;
        [JsonProperty(PropertyName = "headerTitle", NullValueHandling = NullValueHandling.Ignore), Display(Name = "Kopf-Titel", Prompt = "Kopf-Titel der Info"), MaxLength(120, ErrorMessage = "Kopf-Titel zu lang."), Required(ErrorMessage = "Bitte Kopf-Titel eingeben.")]
        public string HeaderTitle { get; set; }
        [JsonProperty(PropertyName = "title", NullValueHandling = NullValueHandling.Ignore), Display(Name = "Titel", Prompt = "Titel der Info"), MaxLength(120, ErrorMessage = "Titel zu lang.")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "subTitle", NullValueHandling = NullValueHandling.Ignore), Display(Name = "Sub-Titel", Prompt = "Zweite Überschrift"), MaxLength(120, ErrorMessage = "Sub-Titel zu lang.")]
        public string SubTitle { get; set; }
        [JsonProperty(PropertyName = "link", NullValueHandling = NullValueHandling.Ignore), MaxLength(120, ErrorMessage = "Link zu lang"), UIHint("url")]
        public string Link { get; set; }
        [JsonProperty(PropertyName = "linkTitle", NullValueHandling = NullValueHandling.Ignore), MaxLength(60, ErrorMessage = "Link-Bezeichnung zu lang.")]
        public string LinkTitle { get; set; }
        [JsonProperty(PropertyName = "infoContent", NullValueHandling = NullValueHandling.Ignore), Display(Name = "Inhalt", Prompt = "Inhalt der Info-Card"), MaxLength(5000, ErrorMessage = "Info zu lang.")]
        public string InfoContent { get; set; }
        [JsonProperty(PropertyName = "isHtmlCode")]
        public Boolean IsHtmlCode { get; set; } = false;
        [JsonProperty(PropertyName = "commentsAllowed")]
        public Boolean CommentsAllowed { get; set; } = false;
        [JsonProperty(PropertyName = "infoLifeTimeInDays", NullValueHandling = NullValueHandling.Ignore), Range(0.0, 100.0, ErrorMessage = "Lebensdauer der Info nicht im gültigen Bereich."), Display(Name = "Lebensdauer der Info", Prompt = "Wie viel Tage soll die Info gespeichert werden? (0 für keine automatische Löschung."), Required(ErrorMessage = "Lebensdauer für die Info eingeben.")]
        public int InfoLifeTimeInDays { get; set; } = 0;
        [JsonProperty(PropertyName = "commentsLifeTimeInDays", NullValueHandling = NullValueHandling.Ignore), Range(0.0, 100.0, ErrorMessage = "Lebensdauer der Kommentare nicht im gültigen Bereich."), Display(Name = "Lebensdauer der Kommentare", Prompt = "Wie viel Tage sollen die Kommentare gespeichert werden? (0 für keine automatische Löschung."), Required(ErrorMessage = "Lebenszeit für die Kommentare eingeben.")]
        public int CommentsLifeTimeInDays { get; set; } = 3;
        [JsonProperty(PropertyName = "authorFirstName", NullValueHandling = NullValueHandling.Ignore), MaxLength(100), Required(ErrorMessage = "Autor bitte eingeben.")]
        public string AuthorFirstName { get; set; }
        [JsonProperty(PropertyName = "authorLastName", NullValueHandling = NullValueHandling.Ignore), MaxLength(100), Required(ErrorMessage = "Autor bitte eingeben.")]
        public string AuthorLastName { get; set; }
    }
}
