using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using MeetUpPlanner.Shared;

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
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation($"GetNotificationSubscriptions");

            IEnumerable<NotificationSubscription> notificationSubscriptions = await _notificationsRepository.GetAllNotificationSubscriptions();

            return new OkObjectResult(notificationSubscriptions);
        }
    }
}
