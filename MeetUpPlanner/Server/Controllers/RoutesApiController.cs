using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl;
using Microsoft.Extensions.Logging;
using MeetUpPlanner.Server.Repositories;
using MeetUpPlanner.Shared;

namespace MeetUpPlanner.Server.Controllers
{
    [Route("api/routes")]
    [ApiController]
    public class RoutesApiController : ControllerBase
    {
        private MeetUpFunctionsConfig _functionsConfig;
        private readonly ILogger<RoutesApiController> _logger;
        private string _routesApiFunc = "https://brave-forest-0d0217a03-45.westeurope.azurestaticapps.net/";
        public const string HEADER_KEYWORD = "x-meetup-keyword";
        public const string HEADER_TENANT = "x-meetup-tenant";



        public RoutesApiController(ILogger<RoutesApiController> logger, MeetUpFunctionsConfig functionsConfig)
        {
            _functionsConfig = functionsConfig;
           _logger = logger;
        }
        [HttpGet("getstring")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<String> GetString([FromHeader(Name = HEADER_TENANT)] string tenant, [FromHeader(Name = HEADER_KEYWORD)] string keyword, string command)
        {
            _logger.LogInformation($"GetString(command = {command})");
            string result = await $"{_routesApiFunc}/api/{command}"
                                    .WithHeader(HEADER_KEYWORD, keyword)
                                    .WithHeader(HEADER_TENANT, tenant)
                                    .GetStringAsync();
            return result;
        }

        [HttpPost("getroutes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<ExtendedRoute>> GetRoutes([FromHeader(Name = HEADER_TENANT)] string tenant, [FromHeader(Name = HEADER_KEYWORD)] string keyword, [FromBody] RouteFilter filter)
        {
            _logger.LogInformation($"GetRoutes()");

            IEnumerable<ExtendedRoute> routes = await $"https://{_routesApiFunc}/api/GetRoutes"
                          .WithHeader(HEADER_KEYWORD, keyword)
                          .WithHeader(HEADER_TENANT, tenant)
                          .PostJsonAsync(filter)
                          .ReceiveJson<IEnumerable<ExtendedRoute>>();
            return routes;
        }

    }
}
