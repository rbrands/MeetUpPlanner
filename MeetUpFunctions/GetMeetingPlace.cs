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
    public class GetMeetingPlace
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<MeetingPlace> _cosmosRepository;

        public GetMeetingPlace(ILogger<GetMeetingPlace> logger, ServerSettingsRepository serverSettingsRepository,
                                                                CosmosDBRepository<MeetingPlace> cosmosRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosRepository = cosmosRepository;
        }

        [FunctionName("GetMeetingPlace")]
        [OpenApiOperation(Summary = "Gets a MeetingPlace",
                          Description = "Reading meeting place for editing. To be able to read MeetingPlace the user keyword must be set as header x-meetup-keyword.")]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(MeetingPlace))]
        [OpenApiParameter("id", Description = "Id of CalendarItem to read.")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetMeetingPlace/{id}")] HttpRequest req, string id)
        {
            _logger.LogInformation("C# HTTP trigger function GetMeetingPlace processed a request.");
            string tenant = req.Headers[Constants.HEADER_TENANT];
            if (String.IsNullOrWhiteSpace(tenant))
            {
                tenant = null;
            }
            ServerSettings serverSettings = await _serverSettingsRepository.GetServerSettings(tenant);
            string keyWord = req.Headers[Constants.HEADER_KEYWORD];
            if (String.IsNullOrEmpty(keyWord) || !serverSettings.IsAdmin(keyWord))
            {
                _logger.LogWarning("GetMeetingPlace called with wrong keyword.");
                return new BadRequestErrorMessageResult("Keyword is missing or wrong.");
            }
            MeetingPlace meetingPlaces = await _cosmosRepository.GetItem(id);
            return new OkObjectResult(meetingPlaces);
        }
    }
}
