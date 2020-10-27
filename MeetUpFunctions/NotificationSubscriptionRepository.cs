using System;
using System.Collections.Generic;
using System.Text;
using MeetUpPlanner.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;

namespace MeetUpPlanner.Functions
{
    public class NotificationSubscriptionRepository
    {
        private IConfiguration _config;
        CosmosDBRepository<NotificationSubscription> _cosmosDbRepository;
        public NotificationSubscriptionRepository(IConfiguration config, CosmosDBRepository<NotificationSubscription> cosmosDBRepository)
        {
            _config = config;
            _cosmosDbRepository = cosmosDBRepository;
        }

        public async Task<NotificationSubscription> WriteNotificationSubscription(NotificationSubscription subscription)
        {
            subscription.LogicalKey = subscription.Url;
            if (!String.IsNullOrWhiteSpace(subscription.Tenant))
            {
                subscription.LogicalKey += "-" + subscription.Tenant;
            }
            subscription.TimeToLive = Constants.SUBSCRIPTION_TTL;

            return await _cosmosDbRepository.UpsertItem(subscription);
        }
    }
}
