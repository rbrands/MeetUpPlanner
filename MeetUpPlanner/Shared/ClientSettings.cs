using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;



namespace MeetUpPlanner.Shared
{
    public class ClientSettings : CosmosDBEntity
    {
        public string Title { get; set; } = "MeetUp-Planner";
        [DataType(DataType.Url, ErrorMessage = "Bitte eine gültige URL eingeben.")]
        [Display(Name = "Link", Prompt = "Optionaler Link für weitere Infos."), UIHint("Url")]
        public string FurtherInfoLink { get; set; }
        [Display(Name = "Titel des Links", Prompt = "Titel zu dem weiterführenden Link."), MaxLength(40, ErrorMessage = "Titel zu dem Link zu lang.")]
        public string FurtherInfoTitle { get; set; }
    }
}
