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
            get { return _clientSettings;  }
            set
            {
                _clientSettings = value;
                NotifyStateChanged();
            }
        }
        [MaxLength(50, ErrorMessage = "Schlüsselwort zu lang.")]
        [Required(ErrorMessage = "Schlüsselwort fehlt.")]
        public string KeyWord { get; set; }
        [MaxLength(50, ErrorMessage = "Privates Schlüsselwort zu lang.")]
        public string PrivateKeyWord1 { get; set; }
        [MaxLength(50, ErrorMessage = "Privates Schlüsselwort zu lang.")]
        public string PrivateKeyWord2 { get; set; }
        [MaxLength(50, ErrorMessage = "Privates Schlüsselwort zu lang.")]
        public string PrivateKeyWord3 { get; set; }
        [MaxLength(120, ErrorMessage = "Der Name ist zu lang."), MinLength(4, ErrorMessage = "Der Name ist zu kurz.") ]
        [Required(ErrorMessage = "Der Name fehlt.")]
        public string Name { get; set; }
        [MaxLength(60, ErrorMessage = "Die Tel-Nr/Mail-Adresse ist zu lang"), MinLength(8, ErrorMessage = "Die Tel-Nr/Mail-Adresse ist zu kurz.")]
        [Required(ErrorMessage = "Tel-Nr/Mail-Adresse fehlen.")]
        public string PhoneMail { get; set; }
        public bool SaveSettings { get; set; } = true;

        public void NotifyStateChanged() => OnChange?.Invoke();

    }
}
