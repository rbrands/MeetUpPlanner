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
    public class CopyWeeklysToNextWeek
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<CalendarItem> _cosmosRepository;

        public CopyWeeklysToNextWeek(ILogger<CopyWeeklysToNextWeek> logger, ServerSettingsRepository serverSettingsRepository, CosmosDBRepository<CalendarItem> cosmosRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosRepository = cosmosRepository;
        }

        /// <summary>
        /// Copies all CalendarItems marked as "weekly" from current day to next week.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("CopyWeeklysToNextWeek")]
        [OpenApiOperation(Summary = "Copies all CalendarItems marked as weekly to next week.",
                          Description = "Copies all CalendarItems marked as weekly from current day to next week.")]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(BackendResult), Description = "Result of operation.")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function CopyWeeklysToNextWeek processed a request.");

            // Get a list of all CalendarItems and filter all applicable ones
            DateTime compareDate = DateTime.Today;
            IEnumerable<CalendarItem> rawListOfCalendarItems = await _cosmosRepository.GetItems(d => d.StartDate > compareDate && d.StartDate < compareDate.AddHours(24.0) && d.Weekly && !d.IsCopiedToNextWeek);
            int counter = 0;
            foreach (CalendarItem cal in rawListOfCalendarItems)
            {
                ++counter;
                // First mark current item as processed
                cal.IsCopiedToNextWeek = true;
                await _cosmosRepository.UpsertItem(cal);
                // Create new item in next week
                cal.Id = null;
                cal.IsCopiedToNextWeek = false;
                cal.IsCanceled = false;
                cal.StartDate = cal.StartDate.AddDays(7.0);
                cal.PublishDate = cal.PublishDate.AddDays(7.0);
                await _cosmosRepository.UpsertItem(cal);
            }

            return new OkObjectResult(new BackendResult(true, $"Copied {counter} weeklys for today"));
        }
    }
}
