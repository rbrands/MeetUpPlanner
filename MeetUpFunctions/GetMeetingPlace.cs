using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MeetUpPlanner.Shared;
using System.Web.Http;

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
