using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MeetUpPlanner.Shared;
using System.Web.Http;
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;


namespace MeetUpPlanner.Functions
{
    public class GetInfoItem
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<InfoItem> _cosmosRepository;

        public GetInfoItem(ILogger<GetInfoItem> logger, ServerSettingsRepository serverSettingsRepository, CosmosDBRepository<InfoItem> cosmosRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosRepository = cosmosRepository;
        }

        [FunctionName("GetInfoItem")]
        [OpenApiOperation(Summary = "Gets the InfoItem by the given itemId",
                          Description = "Reading a InfoItem by id. To be able to read a InfoItem the user keyword must be set as header x-meetup-keyword.")]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(InfoItem))]
        [OpenApiParameter("id", Description = "Id of InfoItem to read.")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetInfoItem/{id}")] HttpRequest req,
            string id)
        {
            _logger.LogInformation($"C# HTTP trigger function GetInfoItem/{id} processed a request.");
            string tenant = req.Headers[Constants.HEADER_TENANT];
            if (String.IsNullOrWhiteSpace(tenant))
            {
                tenant = null;
            }
            ServerSettings serverSettings = await _serverSettingsRepository.GetServerSettings(tenant);
            string keyWord = req.Headers[Constants.HEADER_KEYWORD];
            if (String.IsNullOrEmpty(keyWord) || !serverSettings.IsUser(keyWord))
            {
                return new BadRequestErrorMessageResult("Keyword is missing or wrong.");
            }
            InfoItem item = await _cosmosRepository.GetItem(id);
            return new OkObjectResult(item);
        }
    }
}
