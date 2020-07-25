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
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;


namespace MeetUpPlanner.Functions
{
    public class GetCalendarItem
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<CalendarItem> _cosmosRepository;

        public GetCalendarItem(ILogger<WriteCalendarItem> logger, ServerSettingsRepository serverSettingsRepository, CosmosDBRepository<CalendarItem> cosmosRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosRepository = cosmosRepository;
        }

        [FunctionName("GetCalendarItem")]
        [OpenApiOperation(Summary = "Gets the CalendarItem by the given itemId",
                          Description = "Reading a CalendarItem by id. To be able to read a CalenderItem the user keyword must be set as header x-meetup-keyword.")]
        [OpenApiResponseBody(System.Net.HttpStatusCode.OK, "application/json", typeof(CalendarItem))]
        [OpenApiParameter("id", Description = "Id of CalendarItem to read.")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetCalendarItem/{id}")] HttpRequest req,
            string id)
        {
            _logger.LogInformation($"C# HTTP trigger function GetCalendarItem/{id} processed a request.");
            ServerSettings serverSettings = await _serverSettingsRepository.GetServerSettings();

            string keyWord = req.Headers[Constants.HEADER_KEYWORD];
            if (String.IsNullOrEmpty(keyWord) || !serverSettings.IsUser(keyWord))
            {
                return new BadRequestErrorMessageResult("Keyword is missing or wrong.");
            }
            CalendarItem item = await _cosmosRepository.GetItem(id);
            return new OkObjectResult(item);
        }
    }
}
