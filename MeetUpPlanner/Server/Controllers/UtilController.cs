using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MeetUpPlanner.Server.Repositories;


namespace MeetUpPlanner.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UtilController : ControllerBase
    {
        IMeetUpFunctions _meetUpFunctions;
        private readonly ILogger<UtilController> logger;
        const string serverVersion = "2020-07-02";
        string functionsVersion = "2020-07-03";

        public UtilController(ILogger<UtilController> logger, IMeetUpFunctions meetUpFunctions)
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
            string functionsVersions = await _meetUpFunctions.GetVersion();
            
            return functionsVersion;
        }
    }
}
