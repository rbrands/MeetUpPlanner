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
using MeetUpPlanner.Client.Shared;

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
                    // Calculate/estimate end of event, default is 2 hours
                    End = new CalDateTime(meetUp.StartDate.AddHours(GetEstimatedDurationInHours(meetUp))),
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

        public uint GetTempoAsKilometersPerHour(ExtendedCalendarItem meetUp)
        {
            // Usual formats: '25 - 30km/h', '30km/h', '~20kmh'. We simply parse
            // the first 2-3 digit number and interpret it as km/h. the result
            // is also clamped between 0..40 to guard against unexpected inputs.
            uint kmh = 0;
            try
            {
                var match = Regex.Match(meetUp.Tempo, @"(\d{1,3})");
                if (match.Success)
                {
                    kmh = uint.Parse(match.Groups[1].ToString());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                // in case of any exception ==> fallback to defaults
            }

            return Math.Clamp(kmh, 0, 40);
        }
        public uint GetDistanceAsKilometers(ExtendedCalendarItem meetUp)
        {
            // Usual formats include '65km / 200Hm' with or without spaces, '40
            // oder 60km'. Return value is clamped between 0 and 300 to guard
            // against unexpected inputs.
            uint distance = 0;
            try
            {
                var match = Regex.Match(meetUp.LevelDescription, @"(\d{1,4})\s*(?:km)?");
                if (match.Success)
                {
                    distance = uint.Parse(match.Groups[1].ToString());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                // in case of any exception ==> fallback to defaults
            }

            return Math.Clamp(distance, 0, 300);
        }
        /// <summary>
        /// To get a better fit for the duration (instead of 1 hour as default) this function tries to estimate the duration by parsing the tempo and length.
        /// If no appropriate tempo/distance is detected ==> use 2 hours
        /// Contributed by: Moritz von Göwels https://github.com/the-kenny
        /// </summary>
        /// <param name="meetUp"></param>
        /// <returns></returns>
        public  double GetEstimatedDurationInHours(ExtendedCalendarItem meetUp)
        {
            var distance = GetDistanceAsKilometers(meetUp); // 0..inf
            if (distance == 0) { distance = 50; } // default to 50km

            var speed = GetTempoAsKilometersPerHour(meetUp); // 0..100
            if (speed == 0) { speed = 25; } // Default to 25km/h

            double duration = (double)distance / (double)speed;
            // Clamp between 15 minutes and 24 hours
            return Math.Clamp(duration, 0.25, 24.0);
        }

    }
}
