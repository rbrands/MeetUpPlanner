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
    public class GetExtendedInfoItems
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<InfoItem> _cosmosRepository;
        private CosmosDBRepository<CalendarComment> _commentRepository;

        public GetExtendedInfoItems(ILogger<GetExtendedInfoItems> logger, ServerSettingsRepository serverSettingsRepository,
                                                                          CosmosDBRepository<InfoItem> cosmosRepository,
                                                                          CosmosDBRepository<CalendarComment> commentRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosRepository = cosmosRepository;
            _commentRepository = commentRepository;
        }

        [FunctionName("GetExtendedInfoItems")]
        [OpenApiOperation(Summary = "Gets the relevant ExtendedInfoItems",
                          Description = "Reading current ExtendedCalendarItems (InfoItem including correpondent comments). To be able to read InfoItems the user keyword must be set as header x-meetup-keyword.")]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(IEnumerable<ExtendedInfoItem>))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function GetExtendedInfoItems processed a request.");
            string tenant = req.Headers[Constants.HEADER_TENANT];
            if (String.IsNullOrWhiteSpace(tenant))
            {
                tenant = null;
            }
            ServerSettings serverSettings = await _serverSettingsRepository.GetServerSettings(tenant);
            string keyWord = req.Headers[Constants.HEADER_KEYWORD];
            if (String.IsNullOrEmpty(keyWord) || !serverSettings.IsUser(keyWord))
            {
                _logger.LogWarning("GetExtendedInfoItems called with wrong keyword.");
                return new BadRequestErrorMessageResult("Keyword is missing or wrong.");
            }
            IEnumerable<InfoItem> rawListOfInfoItems;
            if (null == tenant)
            {
                rawListOfInfoItems = await _cosmosRepository.GetItems(d => (d.Tenant ?? String.Empty) == String.Empty);
            }
            else
            {
                rawListOfInfoItems = await _cosmosRepository.GetItems(d => d.Tenant.Equals(tenant));
            }
            List<ExtendedInfoItem> resultInfoItems = new List<ExtendedInfoItem>(10);
            foreach (InfoItem item in rawListOfInfoItems)
            {
                // Create ExtendedInfoItem and get comments
                ExtendedInfoItem extendedItem = new ExtendedInfoItem(item);
                resultInfoItems.Add(extendedItem);
                // Read all comments
                extendedItem.CommentsList = await _commentRepository.GetItems(c => c.CalendarItemId.Equals(extendedItem.Id));
            }
            IEnumerable<ExtendedInfoItem> orderedList = resultInfoItems.OrderBy(d => d.OrderId);
            return new OkObjectResult(orderedList);
        }
    }
}
