using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<IActionResult> WriteCalendarItem([FromQuery] string keyword, [FromBody] CalendarItem calendarItem)
        {
            await _meetUpFunctions.WriteCalendarItem(keyword, calendarItem);
            return Ok();
        }

        [HttpGet("calendaritems")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCalendarItems([FromQuery] string keyword, [FromQuery] string privatekeywords)
        {
            IEnumerable<CalendarItem> calendarItems = await _meetUpFunctions.GetCalendarItems(keyword, privatekeywords);
            return Ok(calendarItems);
        }
        [HttpGet("extendedcalendaritems")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetExtendedCalendarItems([FromQuery] string keyword, [FromQuery] string privatekeywords)
        {
            IEnumerable<ExtendedCalendarItem> calendarItems = await _meetUpFunctions.GetExtendedCalendarItems(keyword, privatekeywords);
            return Ok(calendarItems);
        }
        [HttpGet("calendaritem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCalendarItem([FromQuery] string keyword, [FromQuery] string itemId)
        {
            CalendarItem calendarItem = await _meetUpFunctions.GetCalendarItem(keyword, itemId);
            return Ok(calendarItem);
        }
        [HttpGet("extendedcalendaritem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetExtendedCalendarItem([FromQuery] string keyword, [FromQuery] string itemId)
        {
            ExtendedCalendarItem calendarItem = await _meetUpFunctions.GetExtendedCalendarItem(keyword, itemId);
            return Ok(calendarItem);
        }
        [HttpPost("addparticipant")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddParticipant([FromQuery] string keyword, [FromBody] Participant participant)
        {
            BackendResult result = await _meetUpFunctions.AddParticipantToCalendarItem(keyword, participant);
            return Ok(result);
        }
        [HttpPost("removeparticipant")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveParticipant([FromQuery] string keyword, [FromBody] Participant participant)
        {
            BackendResult result = await _meetUpFunctions.RemoveParticipantFromCalendarItem(keyword, participant);
            return Ok(result);
        }
        [HttpPost("addcomment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddComment([FromQuery] string keyword, [FromBody] CalendarComment comment)
        {
            BackendResult result = await _meetUpFunctions.AddCommentToCalendarItem(keyword, comment);
            return Ok(result);
        }
        [HttpPost("removecomment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RemoveComment([FromQuery] string keyword, [FromBody] CalendarComment comment)
        {
            BackendResult result = await _meetUpFunctions.RemoveCommentFromCalendarItem(keyword, comment);
            return Ok(result);
        }
        [HttpPost("deletecalendaritem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteCalendarItem([FromQuery] string keyword, [FromBody] CalendarItem calendarItem)
        {
            BackendResult result = await _meetUpFunctions.DeleteCalendarItem(keyword, calendarItem);
            return Ok(result);
        }
    }
}
