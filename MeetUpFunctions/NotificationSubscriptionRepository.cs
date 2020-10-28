using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using MeetUpPlanner.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;
using WebPush;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace MeetUpPlanner.Functions
{
    public class NotificationSubscriptionRepository
    {
        private IConfiguration _config;
        private ILogger _logger;
        CosmosDBRepository<NotificationSubscription> _cosmosDbRepository;
        private string _notificationPrivateKey;
        public NotificationSubscriptionRepository(IConfiguration config, ILogger<NotificationSubscription> logger, CosmosDBRepository<NotificationSubscription> cosmosDBRepository)
        {
            _config = config;
            _cosmosDbRepository = cosmosDBRepository;
            _notificationPrivateKey = _config[Constants.NOTIFICATION_KEY_CONFIG];
            _logger = logger;
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

        public async Task NotifyParticipants(ExtendedCalendarItem calendarItem, string firstName, string lastName, string message)
        {
            IEnumerable<NotificationSubscription> subscriptions;
            var publicKey = "BJkgu1ZbFHQm1gQCkYBvsHgZn8-f_v9f9HzIi9UQlCYq2DfUzv4OEx1Dfg9gD0s88fSQ8Ya8kdE4Ib422JHk_E0";
            var privateKey = _notificationPrivateKey;

            var vapidDetails = new VapidDetails("mailto:info@meetupplanner.de", publicKey, privateKey);
            var webPushClient = new WebPushClient();

            _logger.LogInformation($"NotifiyParticpants(<{calendarItem.Title}>, <{firstName}>, <{lastName}>, <{message}>)");
            // Add host as participant to avoid extra handling
            if (!calendarItem.WithoutHost)
            {
                Participant hostAsParticipant = new Participant() { ParticipantFirstName = calendarItem.HostFirstName, ParticipantLastName = calendarItem.HostLastName }               ;
                calendarItem.ParticipantsList = calendarItem.ParticipantsList.Append(hostAsParticipant);
            }
            foreach (Participant p in calendarItem.ParticipantsList)
            {
                _logger.LogInformation($"NotifiyParticpants: Participant {p.ParticipantFirstName} {p.ParticipantLastName} ");
                if (!p.ParticipantFirstName.Equals(firstName) || !p.ParticipantLastName.Equals(lastName))
                {
                    if (null == calendarItem.Tenant)
                    {
                        subscriptions = await _cosmosDbRepository.GetItems(d => d.UserFirstName.Equals(p.ParticipantFirstName) && d.UserLastName.Equals(p.ParticipantLastName) && (d.Tenant ?? String.Empty) == String.Empty);
                    }
                    else
                    {
                        subscriptions = await _cosmosDbRepository.GetItems(d => d.UserFirstName.Equals(p.ParticipantFirstName) && d.UserLastName.Equals(p.ParticipantLastName) && d.Tenant.Equals(calendarItem.Tenant));
                    }
                    _logger.LogInformation($"NotifiyParticpants: {subscriptions.Count()} subscriptions for {p.ParticipantFirstName} {p.ParticipantLastName} ");
                    foreach (NotificationSubscription subscription in subscriptions)
                    {
                        try
                        {
                            var pushSubscription = new PushSubscription(subscription.Url, subscription.P256dh, subscription.Auth);
                            var payload = JsonSerializer.Serialize(new
                            {
                                message = $"{calendarItem.Title}: {message}",
                                url = subscription.PlannerUrl,
                            });
                            _logger.LogInformation($"NotifiyParticpants.SendNotificationAsync({pushSubscription.Endpoint})");
                            await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError("Error sending push notification: " + ex.Message);
                            await _cosmosDbRepository.DeleteItemAsync(subscription.Id);
                        }
                    }
                }
            }

        }

    }
}
