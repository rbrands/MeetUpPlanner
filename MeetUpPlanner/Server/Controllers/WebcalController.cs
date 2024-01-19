using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using System.Text.RegularExpressions;


using MeetUpPlanner.Server.Repositories;
using MeetUpPlanner.Shared;
using Ical.Net.Serialization;
using System.Text;
using System;
using System.Web;

namespace MeetUpPlanner.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WebcalController : ControllerBase
    {
        private readonly MeetUpFunctions _meetUpFunctions;
        private readonly ILogger<WebcalController> _logger;

        public WebcalController(ILogger<WebcalController> logger, MeetUpFunctions meetUpFunctions)
        {
            _logger = logger;
            _meetUpFunctions = meetUpFunctions;
        }

        [HttpGet("getmeetups/{key}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMeetUps([FromRoute] string key)
        {
            var requestUrl = $"{Request.Scheme}://{Request.Host.Value}/";
            TenantClientSettings tenantClientSettings = await _meetUpFunctions.GetTenantClientSettings(requestUrl);
            var meetUps = await _meetUpFunctions.GetExtendedCalendarItems(tenantClientSettings.Tenant.TenantKey, key, null);
            Calendar webCal = new Calendar();
            webCal.AddProperty("X-WR-CALNAME", "MeetUpPlanner");
            webCal.AddTimeZone(new VTimeZone("Europe/Berlin"));
            foreach (var meetUp in meetUps.Where(m => !m.IsCanceled))
            {
                StringBuilder description = new StringBuilder();
                description.AppendLine(meetUp.LevelDescription);
                description.AppendLine(meetUp.Tempo);
                if (meetUp.IsCross)
                {
                    description.Append("Cross ");
                }
                if (meetUp.IsTraining)
                {
                    description.Append("Training ");
                }
                if (meetUp.IsInternal)
                {
                    description.Append("Nur für Vereinsmitglieder");
                }
                description.AppendLine();
                description.Append(StripHTML(meetUp.Summary));
                var icalEvent = new CalendarEvent
                {
                    Summary = meetUp.Title,
                    Description = description.ToString(),
                    Start = new CalDateTime(meetUp.StartDate),
                    End = new CalDateTime(meetUp.StartDate.AddHours(meetUp.GetEstimatedDurationInHours())),
                    Location = meetUp.Place
                };
                webCal.Events.Add(icalEvent);
            }
            var iCalSerializer = new CalendarSerializer();
            string result = iCalSerializer.SerializeToString(webCal);

            return File(Encoding.UTF8.GetBytes(result), "text/calendar", "MeetUpPlanner.ics");
        }
        [HttpGet("getwebcaltoken")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetWebcalToken([FromHeader(Name = "x-meetup-tenant")] string tenant, [FromHeader(Name = "x-meetup-keyword")] string keyword)
        {
            string webcalToken = await _meetUpFunctions.GetWebcalToken(tenant, keyword);
            return Ok(webcalToken);
        }

        public static string StripHTML(string HTMLText, bool decode = true)
        {
            if (null != HTMLText)
            {
                Regex reg = new Regex("<[^>]+>", RegexOptions.IgnoreCase);
                var stripped = reg.Replace(HTMLText, "");
                return decode ? HttpUtility.HtmlDecode(stripped) : stripped;
            }
            else
            {
                return null;
            }
        }

    }
}
