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
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;
using MeetUpPlanner.Shared;
using System.Collections.Generic;

namespace MeetUpPlanner.Functions
{
    public class RemoveParticipantFromCalendarItem
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<Participant> _cosmosRepository;
        private CosmosDBRepository<CalendarItem> _calendarRepository;
        private NotificationSubscriptionRepository _subscriptionRepository;

        public RemoveParticipantFromCalendarItem(ILogger<RemoveParticipantFromCalendarItem> logger,
                                            ServerSettingsRepository serverSettingsRepository,
                                            NotificationSubscriptionRepository subscriptionRepository,
                                            CosmosDBRepository<CalendarItem> calendarRepository,
                                            CosmosDBRepository<Participant> cosmosRepository
                                            )
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosRepository = cosmosRepository;
            _calendarRepository = calendarRepository;
            _subscriptionRepository = subscriptionRepository;
        }

        [FunctionName("RemoveParticipantFromCalendarItem")]
        [OpenApiOperation(Summary = "Removes a participant from the CalendarItem by the given participant id.",
                          Description = "Every participant has a unique id that is used to delete it.")]
        [OpenApiRequestBody("application/json", typeof(Participant), Description = "Participant to be removed.")]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(BackendResult), Description = "Status of operation.")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation($"C# HTTP trigger function RemoveParticipantFromCalendarItem processed a request.");
            string tenant = req.Headers[Constants.HEADER_TENANT];
            if (String.IsNullOrWhiteSpace(tenant))
            {
                tenant = null;
            }
            ServerSettings serverSettings = await _serverSettingsRepository.GetServerSettings(tenant);

            string keyWord = req.Headers[Constants.HEADER_KEYWORD];
            if (String.IsNullOrEmpty(keyWord) || !(serverSettings.IsUser(keyWord) || _serverSettingsRepository.IsInvitedGuest(keyWord)))
            {
                return new BadRequestErrorMessageResult("Keyword is missing or wrong.");
            }
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Participant participant = JsonConvert.DeserializeObject<Participant>(requestBody);
            if (String.IsNullOrEmpty(participant.Id))
            {
                return new OkObjectResult(new BackendResult(false, "Die Id des Teilnehmers fehlt."));
            }
            // Check if there is someone on waiting list who can be promoted now. But only if removed participant is not from waiting list
            if (!participant.IsWaiting)
            {
                IEnumerable<Participant> participants = await _cosmosRepository.GetItems(p => p.CalendarItemId.Equals(participant.CalendarItemId));
                foreach (Participant p in participants)
                {
                    if (!p.Id.Equals(participant.Id) && p.IsWaiting)
                    {
                        p.IsWaiting = false;
                        await _cosmosRepository.UpsertItem(p);
                        CalendarItem calendarItem = await _calendarRepository.GetItem(p.CalendarItemId);
                        await _subscriptionRepository.NotifyParticipant(calendarItem, p, "Das Warten hat sich gelohnt - Du bist jetzt angemeldet.");
                        break; // only the first one from waiting list can be promoted
                    }
                }
            }
            await _cosmosRepository.DeleteItemAsync(participant.Id);

            return new OkObjectResult(new BackendResult(true));
        }
    }
}

