﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using MeetUpPlanner.Shared;
using Microsoft.Extensions.Configuration;
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

            // Check if there is already a subscription
            NotificationSubscription storedSubscription = (await _cosmosDbRepository.GetItems(s => s.Url.Equals(subscription.Url))).FirstOrDefault();
            if (null != storedSubscription)
            {
                subscription.Id = storedSubscription.Id;
            }
            subscription.TimeToLive = Constants.SUBSCRIPTION_TTL;

            return await _cosmosDbRepository.UpsertItem(subscription);
        }

        public async Task<IEnumerable<NotificationSubscription>> GetAllNotificationSubscriptions()
        {
            IEnumerable<NotificationSubscription> subscriptions = await _cosmosDbRepository.GetItems();
            return subscriptions;
        }
        public async Task NotifyParticipant(CalendarItem calendarItem, Participant participant, string message)
        {
            IEnumerable<NotificationSubscription> subscriptions;
            var publicKey = "BJkgu1ZbFHQm1gQCkYBvsHgZn8-f_v9f9HzIi9UQlCYq2DfUzv4OEx1Dfg9gD0s88fSQ8Ya8kdE4Ib422JHk_E0";
            var privateKey = _notificationPrivateKey;

            var vapidDetails = new VapidDetails("mailto:info@meetupplanner.de", publicKey, privateKey);
            var webPushClient = new WebPushClient();
            _logger.LogInformation($"NotifiyParticpant(<{calendarItem.Title}>, <{participant.ParticipantFirstName}>, <{participant.ParticipantLastName}>, <{message}>)");
            if (null == calendarItem.Tenant)
            {
                subscriptions = await _cosmosDbRepository.GetItems(d => d.UserFirstName.Equals(participant.ParticipantFirstName) && d.UserLastName.Equals(participant.ParticipantLastName) && (d.Tenant ?? String.Empty) == String.Empty);
            }
            else
            {
                subscriptions = await _cosmosDbRepository.GetItems(d => d.UserFirstName.Equals(participant.ParticipantFirstName) && d.UserLastName.Equals(participant.ParticipantLastName) && d.Tenant.Equals(calendarItem.Tenant));
            }
            _logger.LogInformation($"NotifiyParticpant: {subscriptions.Count()} subscriptions for {participant.ParticipantFirstName} {participant.ParticipantLastName} ");
            foreach (NotificationSubscription subscription in subscriptions)
            {
                try
                {
                    var pushSubscription = new PushSubscription(subscription.Url, subscription.P256dh, subscription.Auth);
                    var payload = JsonSerializer.Serialize(new
                    {
                        title = calendarItem.Title,
                        message,
                        url = subscription.PlannerUrl,
                    }); ;
                    _logger.LogInformation($"NotifiyParticpant.SendNotificationAsync({pushSubscription.Endpoint})");
                    await webPushClient.SendNotificationAsync(pushSubscription, payload, vapidDetails);
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error sending push notification: " + ex.Message);
                    await _cosmosDbRepository.DeleteItemAsync(subscription.Id);
                }
            }
        }

        public async Task NotifyParticipants(ExtendedCalendarItem calendarItem, string firstName, string lastName, string message)
        {
            var publicKey = "BJkgu1ZbFHQm1gQCkYBvsHgZn8-f_v9f9HzIi9UQlCYq2DfUzv4OEx1Dfg9gD0s88fSQ8Ya8kdE4Ib422JHk_E0";
            var privateKey = _notificationPrivateKey;

            var vapidDetails = new VapidDetails("mailto:info@meetupplanner.de", publicKey, privateKey);
            var webPushClient = new WebPushClient();

            _logger.LogInformation($"NotifiyParticpants(<{calendarItem.Title}>, <{firstName}>, <{lastName}>, <{message}>)");
            // Add host as participant to avoid extra handling
            List<Participant> participants = new List<Participant>(calendarItem.ParticipantsList);
            if (!calendarItem.WithoutHost)
            {
                Participant hostAsParticipant = new Participant() { ParticipantFirstName = calendarItem.HostFirstName, ParticipantLastName = calendarItem.HostLastName }               ;
                participants.Add(hostAsParticipant);
            }
            foreach (Participant p in participants)
            {
                _logger.LogInformation($"NotifiyParticpants: Participant {p.ParticipantFirstName} {p.ParticipantLastName} ");
                if (!p.ParticipantFirstName.Equals(firstName) || !p.ParticipantLastName.Equals(lastName))
                {
                    await NotifyParticipant(calendarItem, p, message);
                }
            }

        }

    }
}
