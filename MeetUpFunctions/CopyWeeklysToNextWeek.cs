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
        /// Copies all CalendarItems marked as "weekly" from current day to next week. x-meetup-keyword must be set to admin keyword.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("CopyWeeklysToNextWeek")]
        [OpenApiOperation(Summary = "Copies all CalendarItems marked as weekly to next week.",
                          Description = "Copies all CalendarItems marked as weekly from current day to next week. x-meetup-keyword must be set to admin keyword.")]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(BackendResult), Description = "Result of operation.")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function WriteCalendarItem processed a request.");
            string tenant = req.Headers[Constants.HEADER_TENANT];
            if (String.IsNullOrWhiteSpace(tenant))
            {
                tenant = null;
            }
            ServerSettings serverSettings;
            if (null == tenant)
            {
                serverSettings = await _serverSettingsRepository.GetServerSettings();
            }
            else
            {
                serverSettings = await _serverSettingsRepository.GetServerSettings(tenant);
            }

            string keyWord = req.Headers[Constants.HEADER_KEYWORD];
            if (String.IsNullOrEmpty(keyWord) || !serverSettings.IsAdmin(keyWord))
            {
                return new BadRequestErrorMessageResult("Keyword is missing or wrong.");
            }
            // Get a list of all CalendarItems and filter all applicable ones
            IEnumerable<CalendarItem> rawListOfCalendarItems;
            DateTime compareDate = DateTime.Today;
            if (null == tenant)
            {
                rawListOfCalendarItems = await _cosmosRepository.GetItems(d => d.StartDate > compareDate && d.StartDate < compareDate.AddHours(24.0) && (d.Tenant ?? String.Empty) == String.Empty && d.Weekly && !d.IsCopiedToNextWeek);
            }
            else
            {
                rawListOfCalendarItems = await _cosmosRepository.GetItems(d => d.StartDate > compareDate && d.StartDate < compareDate.AddHours(24.0) && d.Tenant.Equals(tenant) && d.Weekly && !d.IsCopiedToNextWeek);
            }
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
                cal.StartDate = cal.StartDate.AddDays(7.0);
                cal.PublishDate = cal.PublishDate.AddDays(7.0);
                await _cosmosRepository.UpsertItem(cal);
            }

            return new OkObjectResult(new BackendResult(true, $"Copied {counter} weeklys for today"));
        }
    }
}
