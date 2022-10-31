using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;

using MeetUpPlanner.Server.Repositories;
using MeetUpPlanner.Shared;
using Ical.Net.Serialization;
using System.Text;
using System;

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
            webCal.AddTimeZone(new VTimeZone("Europe/Berlin"));
            foreach (var meetUp in meetUps)
            {
                var icalEvent = new CalendarEvent
                {
                    Summary = meetUp.Title,
                    Description = meetUp.Summary,
                    Start = new CalDateTime(meetUp.StartDate),
                    // Ends 3 hours later.
                    End = new CalDateTime(meetUp.StartDate.AddHours(3.0)),
                    Location = meetUp.Place
                };
                webCal.Events.Add(icalEvent);
            }
            var iCalSerializer = new CalendarSerializer();
            string result = iCalSerializer.SerializeToString(webCal);

            return File(Encoding.ASCII.GetBytes(result), "text/calendar", $"meetupplanner-{tenantClientSettings.Tenant.TrackKey}.ics");
        }
    }
}
