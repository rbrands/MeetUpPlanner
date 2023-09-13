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
    public class RemoveFederation
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<CalendarItem> _cosmosRepository;
        private CosmosDBRepository<Participant> _participantRepository;
        private NotificationSubscriptionRepository _subscriptionRepository;

        public RemoveFederation(ILogger<WriteCalendarItem> logger,
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
        /// Removes the federation from CalendarItem to the database. x-meetup-keyword must be set to admin keyword.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("RemoveFederation")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function RemoveFederation processed a request.");
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
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            CalendarItem calendarItem = JsonConvert.DeserializeObject<CalendarItem>(requestBody);
            if (null != tenant)
            {
                calendarItem.Tenant = tenant;
            }
            CalendarItem oldCalendarItem = null;
            if (!String.IsNullOrEmpty(calendarItem.Id))
            {
                oldCalendarItem = await _cosmosRepository.GetItem(calendarItem.Id);
                // Patching/removing federation makes only sense if the CalendarItem still lives in the database
                await _cosmosRepository.PatchField(calendarItem.Id, "federation", String.Empty);
                calendarItem = await _cosmosRepository.PatchField(calendarItem.Id, "federatedFrom", String.Empty);
            }

            return new OkObjectResult(calendarItem);
        }
    }
}
