﻿using System;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace MeetUpPlanner.Shared
{
    public class CalendarItem : CosmosDBEntity
    {
        [JsonProperty(PropertyName = "title", NullValueHandling = NullValueHandling.Ignore), Display(Name = "Titel", Prompt = "Titel der Veranstaltung"), MaxLength(120, ErrorMessage = "Titel zu lang."), Required(ErrorMessage = "Bitte Titel eingeben.")]
        public string Title { get; set; }
        [JsonProperty(PropertyName = "startDate"), Display(Name = "Start", Prompt = "Bitte Startdatum und -zeit angeben."), Required(ErrorMessage = "Bitte den Beginn der Veranstaltung angeben.")]
        public DateTime StartDate { get; set; }
        [JsonProperty(PropertyName = "publishDate"), Display(Name = "Veröffentlichung")]
        public DateTime PublishDate { get; set; }
        [JsonProperty(PropertyName = "weekly")]
        public Boolean Weekly { get; set; }
        [JsonProperty(PropertyName = "isCopiedToNextWeek")]
        public Boolean IsCopiedToNextWeek { get; set; }
        [JsonProperty(PropertyName = "place"), Display(Name = "Ort", Prompt = "Wo findet die Veranstaltung statt bzw. wo ist der Start"), MaxLength(256), Required(ErrorMessage = "Bitte den Startort angeben.")]
        public string Place { get; set; }

        [JsonProperty(PropertyName = "DirectionsLink"), Display(Name = "Link zum Startpunkt", Prompt = "Link zum Startpunkt der Ausfahrt."), MaxLength(512), UIHint("url")]
        public string DirectionsLink { get; set; }
        [JsonProperty(PropertyName = "hostFirstName", NullValueHandling = NullValueHandling.Ignore), MaxLength(100), Required(ErrorMessage = "Gastgeber bitte eingeben.")]
        public string HostFirstName { get; set; }
        [JsonProperty(PropertyName = "hostLastName", NullValueHandling = NullValueHandling.Ignore), MaxLength(100), Required(ErrorMessage = "Gastgeber bitte eingeben.")]
        public string HostLastName { get; set; }
        [JsonProperty(PropertyName = "hostAdressInfo", NullValueHandling = NullValueHandling.Ignore), MaxLength(100)]
        public string HostAdressInfo { get; set; }
        [JsonProperty(PropertyName = "withoutHost")]
        public Boolean WithoutHost { get; set; } = false;

        [JsonProperty(PropertyName = "summary", NullValueHandling = NullValueHandling.Ignore), Display(Name = "Zusammenfassung", Prompt = "Kurze Zusammenfassung des Termins"), MaxLength(5000, ErrorMessage = "Zusammenfassung zu lang.")]
        public string Summary { get; set; }
        [JsonProperty(PropertyName = "maxRegistrationsCount", NullValueHandling = NullValueHandling.Ignore), Range(0.0, 150.0, ErrorMessage = "Gruppengröße nicht im gültigen Bereich."), Display(Name = "Maximale Anzahl Teilnehmer", Prompt = "Anzahl eingeben"), Required(ErrorMessage = "Max. Anzahl Teilnehmer eingeben")]
        public int MaxRegistrationsCount { get; set; } = 10;
        [JsonProperty(PropertyName = "minRegistrationsCount", NullValueHandling = NullValueHandling.Ignore), Range(0.0, 150.0, ErrorMessage = "Mindestteilnehmerzahl nicht im gültigen Bereich."), Display(Name = "Minimale Anzahl Teilnehmer", Prompt = "Anzahl eingeben")]
        public int MinRegistrationsCount { get; set; } = 0;
        [JsonProperty(PropertyName = "maxWaitingList", NullValueHandling = NullValueHandling.Ignore), Range(0.0, 150.0, ErrorMessage = "Größe der Warteliste nicht im gültigen Bereich."), Display(Name = "Maximale Anzahl auf Warteliste", Prompt = "Anzahl eingeben"), Required(ErrorMessage = "Max. Anzahl auf Warteliste eingeben")]
        public int MaxWaitingList { get; set; } = 0;
        [JsonProperty(PropertyName = "maxCoGuidesCount", NullValueHandling = NullValueHandling.Ignore)]
        public int MaxCoGuidesCount { get; set; } = 0;
        [JsonProperty(PropertyName = "privateKeyword", NullValueHandling = NullValueHandling.Ignore), MaxLength(50, ErrorMessage = "Privates Schlüsselwort zu lang.")]
        public string PrivateKeyword { get; set; }
        [JsonProperty(PropertyName = "isInternal")]
        public Boolean IsInternal { get; set; }
        [JsonProperty(PropertyName = "levelDescription"), MaxLength(60, ErrorMessage = "Angabe zur Länge bitte kürzen.")]
        public string LevelDescription { get; set; }
        [JsonProperty(PropertyName = "tempo"), MaxLength(35, ErrorMessage = "Tempo-Angabe zu lang.")]
        public string Tempo { get; set; }
        [JsonProperty(PropertyName = "subTitle"), MaxLength(512, ErrorMessage = "Untertitel zu lang.")]
        public string SubTitle { get; set; }
        [JsonProperty(PropertyName = "routeLink", NullValueHandling = NullValueHandling.Ignore), MaxLength(256, ErrorMessage = "Link zu lang"), UIHint("url")]
        public string RouteLink { get; set; }
        [JsonProperty(PropertyName = "link", NullValueHandling = NullValueHandling.Ignore), MaxLength(256, ErrorMessage = "Link zu lang"), UIHint("url")]
        public string Link { get; set; }
        [JsonProperty(PropertyName = "linkTitle", NullValueHandling = NullValueHandling.Ignore), MaxLength(120, ErrorMessage = "Link-Bezeichnung zu lang.")]
        public string LinkTitle { get; set; }
        [JsonProperty(PropertyName = "linkImage", NullValueHandling = NullValueHandling.Ignore), MaxLength(512 , ErrorMessage = "Link zu lang"), UIHint("url")]
        public string LinkImage { get; set; }
        [JsonProperty(PropertyName = "isCross")]
        public Boolean IsCross { get; set; } = false;
        [JsonProperty(PropertyName = "isTraining")]
        public Boolean IsTraining { get; set; } = false;
        [JsonProperty(PropertyName = "isKids")]
        public Boolean IsKids { get; set; } = false;
        [JsonProperty(PropertyName = "isYouth")]
        public Boolean IsYouth { get; set; } = false;
        [JsonProperty(PropertyName = "trainer", NullValueHandling = NullValueHandling.Ignore), MaxLength(512, ErrorMessage ="Trainernamen zu lang.")]
        public string Trainer { get; set; }
        [JsonProperty(PropertyName = "isCanceled")]
        public Boolean IsCanceled { get; set; } = false;
        [JsonProperty(PropertyName = "guestScope", NullValueHandling = NullValueHandling.Ignore), MaxLength(60, ErrorMessage = "Gast-Scope zu lang.")]
        [RegularExpression("[a-zA-Z0-9-_]*", ErrorMessage = "Bitte nur Buchstaben und Zahlen für Gast-Scope.")] 
        public string GuestScope { get; set; }
        [JsonProperty(PropertyName = "attachedInfoType", NullValueHandling = NullValueHandling.Ignore), MaxLength(256)]
        public string AttachedInfoType { get; set; }
        [JsonProperty(PropertyName = "attachedInfoKey", NullValueHandling = NullValueHandling.Ignore), MaxLength(256)]
        public string AttachedInfoKey { get; set; }
        [JsonProperty(PropertyName = "federation", NullValueHandling = NullValueHandling.Ignore)]
        public string Federation { get; set; }
        [JsonProperty(PropertyName = "federatedFrom", NullValueHandling = NullValueHandling.Ignore)]
        public string FederatedFrom {  get; set; }
        public Boolean IsFederated()
        {
            return !String.IsNullOrEmpty(FederatedFrom);
        }
        /// <summary>
        /// Constructor with some suggestions about start-time: Schedule for the next day. On Saturdays and Sundays 
        /// StartTime is 10 am otherwise 18 pm
        /// </summary>
        public CalendarItem()
        {
            DateTime startDate = DateTime.Today.AddDays(1.0); // Default is tomorrow
            if ( startDate.DayOfWeek == DayOfWeek.Saturday || startDate.DayOfWeek == DayOfWeek.Sunday)
            {
                // On Saturday and Sunday starttime is 10 am
                this.StartDate = DateTime.Today.AddDays(1.0).AddHours(10.0);
            }
            else
            {
                this.StartDate = DateTime.Today.AddDays(1.0).AddHours(18.0);
            }
            PublishDate = DateTime.Now;
        }
        public string HostDisplayName(int nameDisplayLength)
        {
            StringBuilder sb = new StringBuilder();
            if (!WithoutHost)
            { 
                if (!String.IsNullOrEmpty(HostFirstName))
                { 
                    sb.Append(HostFirstName).Append(" ");
                }
                if (!String.IsNullOrEmpty(HostLastName))
                {
                    int length = nameDisplayLength > 0 ? Math.Min(nameDisplayLength, HostLastName.Length) : HostLastName.Length;
                    sb.Append(HostLastName.Substring(0, length));
                    if (length < HostLastName.Length)
                    {
                        sb.Append('.');
                    }
                }
            }
            return sb.ToString();
        }
        /// <summary>
        /// Returns a string ready to display in (German) UI.
        /// </summary>
        /// <returns></returns>
        public string GetStartDateAsString()
        {
            string[] weekdays = { "So", "Mo", "Di", "Mi", "Do", "Fr", "Sa" };
            string dateString = String.Empty;
            dateString = weekdays[(int)StartDate.DayOfWeek] + ", " + this.StartDate.ToString("dd.MM. HH:mm") + " Uhr";
            return dateString;
        }
        /// <summary>
        /// Returns a string ready to display in (German) UI.
        /// </summary>
        /// <returns></returns>
        public string GetPublishDateAsString()
        {
            string dateString = String.Empty;
            dateString = this.PublishDate.ToLocalTime().ToString("dd.MM. HH:mm") + " Uhr";
            return dateString;
        }
        [JsonIgnore]
        public string DisplayLinkTitle
        {
            get
            {
                string title = this.LinkTitle;
                if (!String.IsNullOrEmpty(this.Link) && String.IsNullOrEmpty(this.LinkTitle))
                {
                    if (this.Link.Contains("komoot"))
                    {
                        title = "Tour auf Komoot";
                    }
                    else if (this.Link.Contains("strava"))
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
