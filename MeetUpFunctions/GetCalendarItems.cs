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
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;

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
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            _logger.LogInformation("C# HTTP trigger function GetCalendarItems processed a request.");
            ServerSettings serverSettings = await _serverSettingsRepository.GetServerSettings();

            string keyWord = req.Headers[Constants.HEADER_KEYWORD];
            if (String.IsNullOrEmpty(keyWord) || !serverSettings.IsUser(keyWord))
            {
                return new BadRequestErrorMessageResult("Keyword is missing or wrong.");
            }
            string privateKeywordsString = req.Query["privatekeywords"];
            string[] privateKeywords = null;
            if (!String.IsNullOrEmpty(privateKeywordsString))
            {
                privateKeywords = privateKeywordsString.Split(';');
            }

            // Get a list of all CalendarItems and filter all applicable ones
            DateTime compareDate = DateTime.Today.AddDays(-2);
            IEnumerable<CalendarItem> rawListOfCalendarItems = await _cosmosRepository.GetItems(d => d.StartDate > compareDate);
            List<CalendarItem> resultCalendarItems = new List<CalendarItem>(10);
            foreach (CalendarItem item in rawListOfCalendarItems)
            {
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
            return new OkObjectResult(resultCalendarItems);
        }
    }
}
