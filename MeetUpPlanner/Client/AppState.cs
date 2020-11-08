using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MeetUpPlanner.Shared;
using System.ComponentModel.DataAnnotations;

namespace MeetUpPlanner.Client
{
    /// <summary>
    /// Use AppState pattern to hold state across all components
    /// </summary>
    public class AppState
    {
        private ClientSettings _clientSettings;

        public event Action OnChange;
        public ClientSettings ClientSettings
        {
            get { return _clientSettings; }
            set
            {
                _clientSettings = value;
                NotifyStateChanged();
            }
        }
        public TenantSettings Tenant { get; set; } = new TenantSettings();
        [MaxLength(50, ErrorMessage = "Schlüsselwort zu lang.")]
        [Required(ErrorMessage = "Schlüsselwort fehlt.")]
        public string KeyWord { get; set; }
        [MaxLength(50, ErrorMessage = "Privates Schlüsselwort zu lang.")]
        public string PrivateKeyWord1 { get; set; }
        [MaxLength(50, ErrorMessage = "Privates Schlüsselwort zu lang.")]
        public string PrivateKeyWord2 { get; set; }
        [MaxLength(50, ErrorMessage = "Privates Schlüsselwort zu lang.")]
        public string PrivateKeyWord3 { get; set; }
        [MaxLength(80, ErrorMessage = "Der Vorname ist zu lang."), MinLength(3, ErrorMessage = "Der Vorname ist zu kurz.") ]
        [Required(ErrorMessage = "Der Vorname fehlt.")]
        public string FirstName { get; set; }
        [MaxLength(80, ErrorMessage = "Der Nachname ist zu lang."), MinLength(3, ErrorMessage = "Der Nachname ist zu kurz.")]
        [Required(ErrorMessage = "Der Nachname fehlt.")]
        public string LastName { get; set; }
        [MaxLength(120, ErrorMessage = "Die Tel-Nr/Mail-Adresse ist zu lang"), MinLength(8, ErrorMessage = "Die Tel-Nr/Mail-Adresse ist zu kurz.")]
        [Required(ErrorMessage = "Tel-Nr/Mail-Adresse fehlen.")]
        public string PhoneMail { get; set; }
        public Boolean NoAddressNeeded { get; set; } = false;
        public bool SaveSettings { get; set; } = true;

        public bool NotificationSubscriptionRequested { get; set; } = false;

        public void NotifyStateChanged() => OnChange?.Invoke();

        public AppState()
        {
            _clientSettings = new ClientSettings();
        }

    }
}
