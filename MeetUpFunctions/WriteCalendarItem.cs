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
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;


namespace MeetUpPlanner.Functions
{
    public class WriteCalendarItem
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<CalendarItem> _cosmosRepository;
        private CosmosDBRepository<Participant> _participantRepository;
        private NotificationSubscriptionRepository _subscriptionRepository;

        public WriteCalendarItem(ILogger<WriteCalendarItem> logger, 
                                 ServerSettingsRepository serverSettingsRepository,
                                 NotificationSubscriptionRepository subscriptionRepository,
                                 CosmosDBRepository<Participant> participantRepository,
                                 CosmosDBRepository<CalendarItem> cosmosRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosRepository = cosmosRepository;
            _participantRepository = participantRepository;
            _subscriptionRepository = subscriptionRepository;
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
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(CalendarItem), Description = "New CalendarItem as written to database.")]
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
            CalendarItem oldCalendarItem = null;
            if (!String.IsNullOrEmpty(calendarItem.Id))
            {
                oldCalendarItem = await _cosmosRepository.GetItem(calendarItem.Id);
            }
            calendarItem = await _cosmosRepository.UpsertItem(calendarItem);
            if (null != oldCalendarItem)
            {
                string message = null;
                // Compare versions and generate message
                if (oldCalendarItem.IsCanceled != calendarItem.IsCanceled)
                {
                    if (calendarItem.IsCanceled)
                    {
                        message = "Abgesagt!";
                    }
                    else
                    {
                        message = "Absage rückgängig gemacht!";
                    }
                } else if (!oldCalendarItem.StartDate.Equals(calendarItem.StartDate))
                {
                    message = "Neue Startzeit!";
                }
                else if (!oldCalendarItem.Title.Equals(calendarItem.Title))
                {
                    message = "Titel geändert;";
                } else if (!oldCalendarItem.Place.Equals(calendarItem.Place))
                {
                    message = "Startort geändert!";
                } else if (!oldCalendarItem.Summary.Equals(calendarItem.Summary))
                {
                    message = "Neue Infos!";
                }
                if (null != message)
                {
                    ExtendedCalendarItem extendedCalendarItem = new ExtendedCalendarItem(calendarItem);
                    // Read all participants for this calendar item
                    extendedCalendarItem.ParticipantsList = await _participantRepository.GetItems(p => p.CalendarItemId.Equals(extendedCalendarItem.Id));
                    await _subscriptionRepository.NotifyParticipants(extendedCalendarItem, extendedCalendarItem.HostFirstName, extendedCalendarItem.HostLastName, message);
                }
            }

            return new OkObjectResult(calendarItem);
        }
    }
}
