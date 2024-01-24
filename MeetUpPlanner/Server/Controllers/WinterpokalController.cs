using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl;
using Flurl.Http.Newtonsoft;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using MeetUpPlanner.Server.Repositories;
using MeetUpPlanner.Shared;

namespace MeetUpPlanner.Server.Controllers
{
    [Route("api/winterpokal")]
    [ApiController]
    public class WinterpokalController : ControllerBase
    {
        private WinterpokalConfig _winterpokalConfig;
        private readonly ILogger<WinterpokalController> _logger;
        public const string HEADER_API_KEY = "api-key";
        public WinterpokalController(ILogger<WinterpokalController> logger, WinterpokalConfig winterpokalConfig)
        {
            _winterpokalConfig = winterpokalConfig;
            _logger = logger;
        }
        [HttpGet("getteam/{teamId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<WinterpokalTeam> GetTeam([FromRoute] string teamId)
        {
            _logger.LogInformation($"GetTeam({teamId})");
            // To get the dynamic from the serializer Newtonsoft.Json has to be used,
            // see https://www.nuget.org/packages/Flurl.Http.Newtonsoft/#readme-body-tab
            dynamic response = await $"{_winterpokalConfig.ApiEndpoint}/api/v1/teams/get/{teamId}.json"
                          .WithHeader("api-token", _winterpokalConfig.ApiKey)
                          .WithSettings(s => { s.JsonSerializer = new NewtonsoftJsonSerializer(); })
                          .GetJsonAsync();
            WinterpokalTeam team = new WinterpokalTeam()
            {
                Id = Convert.ToInt64(teamId)
            };

            if ( response.status != "OK" )
            {
                team.Name = "Team unbekannt";
            }
            else
            {
                team.Id = response.data.team.id;
                team.Name = response.data.team.name;
                team.Description = response.data.team.description;
                team.Link = response.data.team.url;
                IList<Object> teamUsers = response.data.users;
                foreach (dynamic teamUser in teamUsers)
                {
                    WinterpokalUser user = new WinterpokalUser()
                    {
                        Id = teamUser.id,
                        Name = teamUser.name,
                        Entries = teamUser.entries,
                        Points = teamUser.points,
                        Duration = teamUser.duration,
                        Link = teamUser.url
                    };
                    team.Users.Add(user);
                }
            }
            return team;
        }

    }
}
