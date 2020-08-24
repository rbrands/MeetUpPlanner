using System;
using System.IO;
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
    public class WriteCalendarItem
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<CalendarItem> _cosmosRepository;

        public WriteCalendarItem(ILogger<WriteCalendarItem> logger, ServerSettingsRepository serverSettingsRepository, CosmosDBRepository<CalendarItem> cosmosRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosRepository = cosmosRepository;
        }

        /// <summary>
        /// Writes a new or updated CalendarItem to the database. x-meetup-keyword must be set to admin or user keyword.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("WriteCalendarItem")]
        [OpenApiOperation(Summary = "Writes a new or updated CalendarItem to database.",
                          Description = "If the CalendarItem already exists (same id) it it overwritten.")]
        [OpenApiRequestBody("application/json", typeof(CalendarItem), Description = "New CalendarItem to be written.")]
        [OpenApiResponseBody(System.Net.HttpStatusCode.OK, "application/json", typeof(CalendarItem), Description = "New CalendarItem as written to database.")]
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
            if (String.IsNullOrEmpty(keyWord) || !serverSettings.IsUser(keyWord))
            {
                return new BadRequestErrorMessageResult("Keyword is missing or wrong.");
            }
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            CalendarItem calendarItem = JsonConvert.DeserializeObject<CalendarItem>(requestBody);
            System.TimeSpan diffTime = calendarItem.StartDate.Subtract(DateTime.Now);
            calendarItem.TimeToLive = serverSettings.AutoDeleteAfterDays * 24 * 3600 + (int)diffTime.TotalSeconds;
            if (null != tenant)
            {
                calendarItem.Tenant = tenant;
            }
            calendarItem = await _cosmosRepository.UpsertItem(calendarItem);

            return new OkObjectResult(calendarItem);
        }
    }
}
