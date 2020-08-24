using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Web.Http;
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using MeetUpPlanner.Shared;
using System.Collections.Generic;

namespace MeetUpPlanner.Functions
{
    public class AddCommentToCalendarItem
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<CalendarComment> _cosmosRepository;
        private CosmosDBRepository<CalendarItem> _calendarRepository;
        public AddCommentToCalendarItem(ILogger<AddCommentToCalendarItem> logger,
                                            ServerSettingsRepository serverSettingsRepository,
                                            CosmosDBRepository<CalendarComment> cosmosRepository,
                                            CosmosDBRepository<CalendarItem> calendarRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosRepository = cosmosRepository;
            _calendarRepository = calendarRepository;
        }

        [FunctionName("AddCommentToCalendarItem")]
        [OpenApiOperation(Summary = "Add a comment to the referenced CalendarItem.",
                          Description = "If the CalendarComment already exists (same id) it is overwritten.")]
        [OpenApiRequestBody("application/json", typeof(CalendarComment), Description = "New CalendarComment to be written.")]
        [OpenApiResponseBody(System.Net.HttpStatusCode.OK, "application/json", typeof(BackendResult), Description = "Status of operation.")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation($"C# HTTP trigger function AddCommentToCalendarItem processed a request.");
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
            CalendarComment comment = JsonConvert.DeserializeObject<CalendarComment>(requestBody);
            // Get and check corresponding CalendarItem
            if (String.IsNullOrEmpty(comment.CalendarItemId))
            {
                return new OkObjectResult(new BackendResult(false, "Terminangabe fehlt."));
            }
            CalendarItem calendarItem = await _calendarRepository.GetItem(comment.CalendarItemId);
            if (null == calendarItem)
            {
                return new OkObjectResult(new BackendResult(false, "Angegebenen Termin nicht gefunden."));
            }
            // Set TTL for comment the same as for CalendarItem
            System.TimeSpan diffTime = calendarItem.StartDate.Subtract(DateTime.Now);
            comment.TimeToLive = serverSettings.AutoDeleteAfterDays * 24 * 3600 + (int)diffTime.TotalSeconds;
            // Checkindate to track bookings
            comment.CommentDate = DateTime.Now;
            if (!String.IsNullOrWhiteSpace(tenant))
            { 
                comment.Tenant = tenant;
            }

            comment = await _cosmosRepository.UpsertItem(comment);
            BackendResult result = new BackendResult(true);

            return new OkObjectResult(result);
        }
    }
}
