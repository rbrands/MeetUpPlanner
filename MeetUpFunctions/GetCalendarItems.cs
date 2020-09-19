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
    public class GetCalendarItems
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<CalendarItem> _cosmosRepository;

        public GetCalendarItems(ILogger<GetCalendarItems> logger, ServerSettingsRepository serverSettingsRepository, CosmosDBRepository<CalendarItem> cosmosRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosRepository = cosmosRepository;
        }

        [FunctionName("GetCalendarItems")]
        [OpenApiOperation(Summary = "Gets the relevant CalendarIitems",
                          Description = "Reading current CalendarItems starting in the future or the configured past (in hours). To be able to read CalenderItems the user keyword must be set as header x-meetup-keyword.")]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(IEnumerable<CalendarItem>))]
        [OpenApiParameter("privatekeywords", Description = "Holds a list of private keywords, separated by ;")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function GetCalendarItems processed a request.");
            string tenant = req.Headers[Constants.HEADER_TENANT];
            if (String.IsNullOrWhiteSpace(tenant))
            {
                tenant = null;
            }
            ServerSettings serverSettings = await _serverSettingsRepository.GetServerSettings(tenant);
            string keyWord = req.Headers[Constants.HEADER_KEYWORD];
            if (String.IsNullOrEmpty(keyWord) || !serverSettings.IsUser(keyWord))
            {
                _logger.LogWarning("GetCalendarItems called with wrong keyword.");
                return new BadRequestErrorMessageResult("Keyword is missing or wrong.");
            }
            string privateKeywordsString = req.Query["privatekeywords"];
            string[] privateKeywords = null;
            if (!String.IsNullOrEmpty(privateKeywordsString))
            {
                privateKeywords = privateKeywordsString.Split(';');
            }

            // Get a list of all CalendarItems and filter all applicable ones
            DateTime compareDate = DateTime.Now.AddHours((-serverSettings.CalendarItemsPastWindowHours));

            IEnumerable<CalendarItem> rawListOfCalendarItems;
            if (null == tenant)
            {
                rawListOfCalendarItems = await _cosmosRepository.GetItems(d => d.StartDate > compareDate && (d.Tenant ?? String.Empty) == String.Empty);
            }
            else
            {
                rawListOfCalendarItems = await _cosmosRepository.GetItems(d => d.StartDate > compareDate && d.Tenant.Equals(tenant));
            }
            List<CalendarItem> resultCalendarItems = new List<CalendarItem>(10);
            foreach (CalendarItem item in rawListOfCalendarItems)
            {
                if (!serverSettings.IsAdmin(keyWord) && item.PublishDate.CompareTo(DateTime.UtcNow) > 0)
                {
                    // If calendar item is not ready for publishing skip it
                    continue;
                }
                if ( String.IsNullOrEmpty(item.PrivateKeyword))
                {
                    // No private keyword for item ==> use it
                    resultCalendarItems.Add(item);
                } else if (null != privateKeywords)
                {
                    // Private keyword for item ==> check given keyword list against it
                    foreach (string keyword in privateKeywords)
                    {
                        if (keyword.Equals(item.PrivateKeyword))
                        {
                            resultCalendarItems.Add(item);
                            break;
                        }
                    }
                }
            }
            IEnumerable<CalendarItem> orderedList = resultCalendarItems.OrderBy(d => d.StartDate);
            return new OkObjectResult(orderedList);
        }
    }
}
