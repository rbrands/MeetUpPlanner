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
    public class GetExtendedInfoItem
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<InfoItem> _cosmosRepository;
        private CosmosDBRepository<CalendarComment> _commentRepository;

        public GetExtendedInfoItem(ILogger<GetExtendedCalendarItem> logger,
                                       ServerSettingsRepository serverSettingsRepository,
                                       CosmosDBRepository<InfoItem> cosmosRepository,
                                       CosmosDBRepository<CalendarComment> commentRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosRepository = cosmosRepository;
            _commentRepository = commentRepository;
        }

        [FunctionName("GetExtendedInfoItem")]
        [OpenApiOperation(Summary = "Gets the InfoUtem with all referencing data",
                          Description = "Reading given InfoItem. To be able to read InfoItem the user keyword must be set as header x-meetup-keyword.")]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(ExtendedInfoItem))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetExtendedInfoItem/{id}")] HttpRequest req, string id)
        {
            _logger.LogInformation("C# HTTP trigger function GetExtendedInfoItem processed a request.");
            string tenant = req.Headers[Constants.HEADER_TENANT];
            if (String.IsNullOrWhiteSpace(tenant))
            {
                tenant = null;
            }
            ServerSettings serverSettings = await _serverSettingsRepository.GetServerSettings(tenant);
            string keyWord = req.Headers[Constants.HEADER_KEYWORD];
            if (String.IsNullOrEmpty(keyWord) || !(serverSettings.IsUser(keyWord)))
            {
                _logger.LogWarning("GetExtendedInfoItem called with wrong keyword.");
                return new BadRequestErrorMessageResult("Keyword is missing or wrong.");
            }
            // Get CalendarItem by id
            InfoItem rawInfoItem = await _cosmosRepository.GetItem(id);
            if (null == rawInfoItem)
            {
                return new BadRequestErrorMessageResult("No InfoItem with given id found.");
            }
            ExtendedInfoItem extendedItem = new ExtendedInfoItem(rawInfoItem);
            // Read all comments
            extendedItem.CommentsList = await _commentRepository.GetItems(c => c.CalendarItemId.Equals(extendedItem.Id));
            return new OkObjectResult(extendedItem);
        }
    }
}
