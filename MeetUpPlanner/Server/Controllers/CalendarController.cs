using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MeetUpPlanner.Server.Repositories;
using MeetUpPlanner.Shared;

namespace MeetUpPlanner.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly MeetUpFunctions _meetUpFunctions;
        private readonly ILogger<CalendarController> logger;
        public CalendarController(ILogger<CalendarController> logger, MeetUpFunctions meetUpFunctions)
        {
            _meetUpFunctions = meetUpFunctions;
            this.logger = logger;
        }
        [HttpPost("writecalendaritem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> WriteCalendarItem([FromHeader(Name = "x-meetup-tenant")] string tenant, [FromHeader(Name = "x-meetup-keyword")] string keyword, [FromBody] CalendarItem calendarItem)
        {
            await _meetUpFunctions.WriteCalendarItem(tenant, keyword, calendarItem);
            return Ok();
        }

        [HttpGet("calendaritems")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCalendarItems([FromHeader(Name = "x-meetup-tenant")] string tenant, [FromHeader(Name = "x-meetup-keyword")] string keyword, [FromQuery] string privatekeywords)
        {
            IEnumerable<CalendarItem> calendarItems = await _meetUpFunctions.GetCalendarItems(tenant, keyword, privatekeywords);
            return Ok(calendarItems);
        }
        [HttpGet("extendedcalendaritems")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetExtendedCalendarItems([FromHeader(Name = "x-meetup-tenant")] string tenant, [FromHeader(Name = "x-meetup-keyword")] string keyword, [FromQuery] string privatekeywords)
        {
            IEnumerable<ExtendedCalendarItem> calendarItems = await _meetUpFunctions.GetExtendedCalendarItems(tenant, keyword, privatekeywords);
            return Ok(calendarItems);
        }
        [HttpGet("extendedcalendaritemsfordate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetExtendedCalendarItemsForDate([FromHeader(Name = "x-meetup-tenant")] string tenant, [FromHeader(Name = "x-meetup-keyword")] string keyword, [FromQuery] string privatekeywords, [FromQuery] string requesteddate)
        {
            IEnumerable<ExtendedCalendarItem> calendarItems = await _meetUpFunctions.GetExtendedCalendarItemsForDate(tenant, keyword, privatekeywords, requesteddate);
            return Ok(calendarItems);
        }
        [HttpGet("scopedcalendaritems")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetScopedCalendarItems([FromHeader(Name = "x-meetup-tenant")] string tenant, [FromQuery] string scope)
        {
            IEnumerable<ExtendedCalendarItem> calendarItems = await _meetUpFunctions.GetScopedCalendarItems(tenant, scope);
            return Ok(calendarItems);
        }
        [HttpGet("calendaritem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCalendarItem([FromHeader(Name = "x-meetup-tenant")] string tenant, [FromHeader(Name = "x-meetup-keyword")] string keyword, [FromQuery] string itemId)
        {
            CalendarItem calendarItem = await _meetUpFunctions.GetCalendarItem(tenant, keyword, itemId);
            return Ok(calendarItem);
        }
        [HttpGet("extendedcalendaritem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetExtendedCalendarItem([FromHeader(Name = "x-meetup-tenant")] string tenant, [FromHeader(Name = "x-meetup-keyword")] string keyword, [FromQuery] string itemId)
        {
            ExtendedCalendarItem calendarItem = await _meetUpFunctions.GetExtendedCalendarItem(tenant, keyword, itemId);
            return Ok(calendarItem);
        }
        [HttpGet("extendedcalendaritemforguest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetExtendedCalendarItemForGuest([FromHeader(Name = "x-meetup-tenant")] string tenant, string itemId)
        {
            ExtendedCalendarItem calendarItem = await _meetUpFunctions.GetExtendedCalendarItem(tenant, _meetUpFunctions.InviteGuestKey, itemId);
            return Ok(calendarItem);
        }
        [HttpPost("addparticipant")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddParticipant([FromHeader(Name = "x-meetup-tenant")] string tenant, [FromHeader(Name = "x-meetup-keyword")] string keyword, [FromBody] Participant participant)
        {
            BackendResult result = await _meetUpFunctions.AddParticipantToCalendarItem(tenant, keyword, participant);
            return Ok(result);
        }
        [HttpPost("addguest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddGuest([FromHeader(Name = "x-meetup-tenant")] string tenant, [FromBody] Participant participant)
        {
            BackendResult result = await _meetUpFunctions.AddParticipantToCalendarItem(tenant, _meetUpFunctions.InviteGuestKey, participant);
            return Ok(result);
        }
        [HttpPost("removeparticipant")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveParticipant([FromHeader(Name = "x-meetup-tenant")] string tenant, [FromHeader(Name = "x-meetup-keyword")] string keyword, [FromBody] Participant participant)
        {
            BackendResult result = await _meetUpFunctions.RemoveParticipantFromCalendarItem(tenant, keyword, participant);
            return Ok(result);
        }
        [HttpPost("assignnewhost")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AssignNewHost([FromHeader(Name = "x-meetup-tenant")] string tenant, [FromHeader(Name = "x-meetup-keyword")] string keyword, [FromBody] Participant participant)
        {
            BackendResult result = await _meetUpFunctions.AssignNewHostToCalendarItem(tenant, keyword, participant);
            return Ok(result);
        }
        [HttpPost("removeguest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveGuest([FromHeader(Name = "x-meetup-tenant")] string tenant, [FromBody] Participant participant)
        {
            BackendResult result = await _meetUpFunctions.RemoveParticipantFromCalendarItem(tenant, _meetUpFunctions.InviteGuestKey, participant);
            return Ok(result);
        }
        [HttpPost("addcomment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddComment([FromHeader(Name = "x-meetup-tenant")] string tenant, [FromHeader(Name = "x-meetup-keyword")] string keyword, [FromBody] CalendarComment comment)
        {
            BackendResult result = await _meetUpFunctions.AddCommentToCalendarItem(tenant, keyword, comment);
            return Ok(result);
        }
        [HttpPost("removecomment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveComment([FromHeader(Name = "x-meetup-tenant")] string tenant, [FromHeader(Name = "x-meetup-keyword")] string keyword, [FromBody] CalendarComment comment)
        {
            BackendResult result = await _meetUpFunctions.RemoveCommentFromCalendarItem(tenant, keyword, comment);
            return Ok(result);
        }
        [HttpPost("deletecalendaritem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteCalendarItem([FromHeader(Name = "x-meetup-tenant")] string tenant, [FromHeader(Name = "x-meetup-keyword")] string keyword, [FromBody] CalendarItem calendarItem)
        {
            BackendResult result = await _meetUpFunctions.DeleteCalendarItem(tenant, keyword, calendarItem);
            return Ok(result);
        }
        [HttpPost("requesttrackingreport")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportTrackingReport([FromHeader(Name = "x-meetup-tenant")] string tenant, [FromHeader(Name = "x-meetup-keyword")] string keyword, [FromBody] TrackingReportRequest request)
        {
            TrackingReport report = await _meetUpFunctions.ExportTrackingReport(tenant, keyword, request);
            return Ok(report);
        }

        [HttpGet("getexportlog")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetExportLog([FromHeader(Name = "x-meetup-tenant")] string tenant, [FromHeader(Name = "x-meetup-keyword")] string keyword)
        {
            IEnumerable<ExportLogItem> result = await _meetUpFunctions.GetExportLog(tenant, keyword);
            return Ok(result);
        }

    }
}
