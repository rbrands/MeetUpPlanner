using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Newtonsoft.Json;
using MeetUpPlanner.Shared;
using System.Web.Http;
using System.Linq;
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;

namespace MeetUpPlanner.Functions
{
    public class GetMeetingPlaces
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<MeetingPlace> _cosmosRepository;

        public GetMeetingPlaces(ILogger<GetMeetingPlaces> logger, ServerSettingsRepository serverSettingsRepository,
                                                                          CosmosDBRepository<MeetingPlace> cosmosRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosRepository = cosmosRepository;
        }

        [FunctionName("GetMeetingPlaces")]
        [OpenApiOperation(Summary = "Gets all MeetingPlaces",
                          Description = "Reading all preconfigured meeting places. To be able to read InfoItems the user keyword must be set as header x-meetup-keyword.")]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(IEnumerable<MeetingPlace>))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function GetMeetingPlaces processed a request.");
            string tenant = req.Headers[Constants.HEADER_TENANT];
            if (String.IsNullOrWhiteSpace(tenant))
            {
                tenant = null;
            }
            ServerSettings serverSettings = await _serverSettingsRepository.GetServerSettings(tenant);
            string keyWord = req.Headers[Constants.HEADER_KEYWORD];
            if (String.IsNullOrEmpty(keyWord) || !serverSettings.IsUser(keyWord))
            {
                _logger.LogWarning("GetMeetingPlaces called with wrong keyword.");
                return new BadRequestErrorMessageResult("Keyword is missing or wrong.");
            }
            IEnumerable<MeetingPlace> meetingPlaces;
            if (null == tenant)
            {
                meetingPlaces = await _cosmosRepository.GetItems(d => (d.Tenant ?? String.Empty) == String.Empty);
            }
            else
            {
                meetingPlaces = await _cosmosRepository.GetItems(d => d.Tenant.Equals(tenant));
            }
            return new OkObjectResult(meetingPlaces);
        }
    }
}
