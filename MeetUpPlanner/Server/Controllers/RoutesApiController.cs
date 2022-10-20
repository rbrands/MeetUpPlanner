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
        //private string _routesApiFunc = "https://brave-forest-0d0217a03-48.westeurope.azurestaticapps.net";
        public const string HEADER_KEYWORD = "x-meetup-keyword";
        public const string HEADER_TENANT = "x-meetup-tenant";

        public RoutesApiController(ILogger<RoutesApiController> logger, MeetUpFunctionsConfig functionsConfig)
        {
            _functionsConfig = functionsConfig;
           _logger = logger;
        }
        [HttpGet("getversion")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<String> GetVersion([FromHeader(Name = HEADER_TENANT)] string tenant, [FromHeader(Name = HEADER_KEYWORD)] string keyword)
        {
            _logger.LogInformation($"GetVersion()");
            string result = await $"{_functionsConfig.RoutesApiUrl}/api/GetVersion"
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

            IEnumerable<ExtendedRoute> routes = await $"{_functionsConfig.RoutesApiUrl}/api/GetRoutes"
                          .WithHeader(HEADER_KEYWORD, keyword)
                          .WithHeader(HEADER_TENANT, tenant)
                          .PostJsonAsync(filter)
                          .ReceiveJson<IEnumerable<ExtendedRoute>>();
            return routes;
        }

        [HttpGet("gettagsets")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IEnumerable<TagSet>> GetTagSets([FromHeader(Name = HEADER_TENANT)] string tenant, [FromHeader(Name = HEADER_KEYWORD)] string keyword)
        {
            _logger.LogInformation($"GetTagSets()");

            IEnumerable<TagSet> tagSets = await $"{_functionsConfig.RoutesApiUrl}/api/GetTagSets"
                          .WithHeader(HEADER_KEYWORD, keyword)
                          .WithHeader(HEADER_TENANT, tenant)
                          .GetJsonAsync<IEnumerable<TagSet>>();
            return tagSets;
        }
        [HttpPost("writecomment")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<Comment> WriteComment([FromHeader(Name = HEADER_TENANT)] string tenant, [FromHeader(Name = HEADER_KEYWORD)] string keyword, [FromBody] Comment comment)
        {
            _logger.LogInformation($"WriteComment()");

            Comment updatedComment = await $"{_functionsConfig.RoutesApiUrl}/api/WriteComment"
                          .WithHeader(HEADER_KEYWORD, keyword)
                          .WithHeader(HEADER_TENANT, tenant)
                          .PostJsonAsync(comment)
                          .ReceiveJson<Comment>();
            return updatedComment;
        }

    }
}
