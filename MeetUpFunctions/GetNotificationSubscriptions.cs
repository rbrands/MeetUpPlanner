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
    public class GetNotificationSubscriptions
    {
        private readonly ILogger _logger;
        private NotificationSubscriptionRepository _notificationsRepository;

        public GetNotificationSubscriptions(ILogger<GetNotificationSubscriptions> logger, NotificationSubscriptionRepository notificationsRepository)
        {
            _logger = logger;
            _notificationsRepository = notificationsRepository;
        }

        [FunctionName("GetNotificationSubscriptions")]
        [OpenApiOperation(Summary = "Gets all active notification subscriptions",
                          Description = "Reading all notification subscriptions currently stored..")]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(IEnumerable<NotificationSubscription>))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation($"GetNotificationSubscriptions");

            IEnumerable<NotificationSubscription> notificationSubscriptions = await _notificationsRepository.GetAllNotificationSubscriptions();

            return new OkObjectResult(notificationSubscriptions);
        }
    }
}
