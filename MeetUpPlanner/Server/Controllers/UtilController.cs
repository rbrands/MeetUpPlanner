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
    public class UtilController : ControllerBase
    {
        private readonly MeetUpFunctions _meetUpFunctions;
        private readonly ILogger<UtilController> logger;
        const string serverVersion = "2020-07-28";
        string functionsVersion = "2020-07-03";

        public UtilController(ILogger<UtilController> logger, MeetUpFunctions meetUpFunctions)
        {
            _meetUpFunctions = meetUpFunctions;
            this.logger = logger;
        }

        [HttpGet("version")]
        public String GetVersion()
        {
            logger.LogInformation("Server version returned: {serverVersion}", serverVersion);
            return serverVersion;
        }

        [HttpGet("functionsVersion")]
        public async Task<String> GetFunctionsVersion()
        {
            logger.LogInformation("Functions version returned: {functionsVersion}", functionsVersion);
            functionsVersion = await _meetUpFunctions.GetVersion();
            
            return functionsVersion;
        }

        [HttpGet("clientsettings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetClientSettings()
        {
            ClientSettings clientSettings = await _meetUpFunctions.GetClientSettings();
            return Ok(clientSettings);
        }

        [HttpGet("checkkeyword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckKeyword(string keyword)
        {
            KeywordCheck keywordCheck = await _meetUpFunctions.CheckKeyword(keyword);

            return Ok(keywordCheck);
        }

        [HttpGet("serversettings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetServerSettings([FromQuery] string adminKeyword)
        {
            ServerSettings serverSettings = await _meetUpFunctions.GetServerSettings(adminKeyword);
            return Ok(serverSettings);
        }
        [HttpPost("writeserversettings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> WriteServerSettings([FromQuery] string adminKeyword, [FromBody] ServerSettings serverSettings)
        {
            await _meetUpFunctions.WriteServerSettings(adminKeyword, serverSettings);
            return Ok();
        }
        [HttpPost("writesettings")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> WriteClientSettings([FromQuery] string adminKeyword, [FromBody] ClientSettings clientSettings)
        {
            await _meetUpFunctions.WriteClientSettings(adminKeyword, clientSettings);
            return Ok();
        }
    }
}
