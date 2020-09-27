using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MeetUpPlanner.Server.Repositories;
using MeetUpPlanner.Shared;

namespace MeetUpPlanner.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase
    {
        private readonly MeetUpFunctions _meetUpFunctions;
        private readonly ILogger<InfoController> logger;
        public InfoController(ILogger<InfoController> logger, MeetUpFunctions meetUpFunctions)
        {
            _meetUpFunctions = meetUpFunctions;
            this.logger = logger;
        }
        [HttpPost("writeinfoitem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> WriteInfoItem([FromHeader(Name = "x-meetup-tenant")] string tenant, [FromHeader(Name = "x-meetup-keyword")] string keyword, [FromBody] InfoItem infoItem)
        {
            await _meetUpFunctions.WriteInfoItem(tenant, keyword, infoItem);
            return Ok();
        }
        [HttpGet("extendedinfoitems")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetExtendedInfoItems([FromHeader(Name = "x-meetup-tenant")] string tenant, [FromHeader(Name = "x-meetup-keyword")] string keyword)
        {
            IEnumerable<ExtendedInfoItem> infoItems = await _meetUpFunctions.GetExtendedInfoItems(tenant, keyword);
            return Ok(infoItems);
        }
        [HttpGet("infoitem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetInfoItem([FromHeader(Name = "x-meetup-tenant")] string tenant, [FromHeader(Name = "x-meetup-keyword")] string keyword, [FromQuery] string itemId)
        {
            InfoItem infoItem = await _meetUpFunctions.GetInfoItem(tenant, keyword, itemId);
            return Ok(infoItem);
        }
        [HttpPost("deleteinfoitem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> DeleteInfoItem([FromHeader(Name = "x-meetup-tenant")] string tenant, [FromHeader(Name = "x-meetup-keyword")] string keyword, [FromBody] InfoItem infoItem)
        {
            BackendResult result = await _meetUpFunctions.DeleteInfoItem(tenant, keyword, infoItem);
            return Ok(result);
        }
        [HttpGet("extendedinfoitem")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetExtendedInfoItem([FromHeader(Name = "x-meetup-tenant")] string tenant, [FromHeader(Name = "x-meetup-keyword")] string keyword, [FromQuery] string itemId)
        {
            ExtendedInfoItem infoItem = await _meetUpFunctions.GetExtendedInfoItem(tenant, keyword, itemId);
            return Ok(infoItem);
        }
        [HttpPost("addcomment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddComment([FromHeader(Name = "x-meetup-tenant")] string tenant, [FromHeader(Name = "x-meetup-keyword")] string keyword, [FromBody] CalendarComment comment)
        {
            BackendResult result = await _meetUpFunctions.AddCommentToInfoItem(tenant, keyword, comment);
            return Ok(result);
        }

    }
}
