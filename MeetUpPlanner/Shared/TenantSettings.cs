using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;


namespace MeetUpPlanner.Shared
{
    public class TenantSettings : CosmosDBEntity
    {
        [Required(ErrorMessage = "Bitte Bezeichnung für den Tenant eingeben"), MaxLength(180, ErrorMessage = "Tenantbezeichnung bitte kürzer als 180 Zeichen.")]
        public string TenantName { get; set; }
        [MaxLength(80, ErrorMessage = "Tenant-Key bitte kürzer als 80 Zeichen.")]
        public string TenantKey { get; set; }
        [MaxLength(80, ErrorMessage = "Track-Key bitte kürzer als 80 Zeichen.")]
        public string TrackKey { get; set; }
        [Required(ErrorMessage = "Bitte Admin-Rolle angeben"), MaxLength(80, ErrorMessage = "Admin-Rolle bitte kürzer als 80 Zeichen.")]
        [RegularExpression("[a-zA-Z0-9-_]*", ErrorMessage = "Bitte nur Buchstaben und Zahlen für Admin-Rolle.")]
        public string AdminRole { get; set; }
        [Required(ErrorMessage = "Primary URL erforderlich."), MaxLength(180, ErrorMessage = "Primary URL bitte kürzer als 180 Zeichen.")]
        [RegularExpression(@"^https\:\/\/[0-9a-z]([-.\w]*[0-9a-z])*(:(0-9)*)*(\/?)([a-z0-9\-\.\?\,\'\/\\\+&%\$#_]*)?$", ErrorMessage = "Bitte eine gültige URL https://ausfahrten ... eingeben")]
        public string PrimaryUrl { get; set; }
        public Boolean ClubMemberShipAllowed { get; set; } = true;
        public Boolean GuestNameShown { get; set; } = true;
        public Boolean IsLocked { get; set; } = false;
        public string LockMessage { get; set; }
        public Boolean TracksEnabled { get; set; } = true;
        public Boolean LocalAdministrationDisabled { get; set; } = false;

        public TenantSettings()
        {
        }
        public TenantSettings(string primaryUrl, string name, bool clubMembershipAllowed, bool guestNameShown)
        {
            PrimaryUrl = primaryUrl;
            TenantKey = name;
            ClubMemberShipAllowed = clubMembershipAllowed;
            GuestNameShown = guestNameShown;
        }

    }
}
