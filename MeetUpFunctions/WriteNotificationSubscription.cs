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
    public class WriteNotificationSubscription
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private NotificationSubscriptionRepository _subscriptionRepository;

        public WriteNotificationSubscription(ILogger<WriteNotificationSubscription> logger, ServerSettingsRepository serverSettingsRepository, NotificationSubscriptionRepository subscriptionRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _subscriptionRepository = subscriptionRepository;
        }

        /// <summary>
        /// Writes a new or updated CalendarItem to the database. x-meetup-keyword must be set to admin or user keyword.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("WriteNotificationSubscription")]
        [OpenApiOperation(Summary = "Writes a new or updated NotificationSubscription to database.",
                          Description = "If the NotificationSubscription already exists (same tenant + url) it it overwritten.")]
        [OpenApiRequestBody("application/json", typeof(NotificationSubscription), Description = "New NotificationSubscription to be written.")]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(NotificationSubscription), Description = "New NotificationSubscription as to be written to database.")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function NotificationSubscription processed a request.");
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
            NotificationSubscription notificationSubscription = JsonConvert.DeserializeObject<NotificationSubscription>(requestBody);
            if (null != tenant)
            {
                notificationSubscription.Tenant = tenant;
            }
            notificationSubscription = await _subscriptionRepository.WriteNotificationSubscription(notificationSubscription);

            return new OkObjectResult(notificationSubscription);
        }
    }
}
