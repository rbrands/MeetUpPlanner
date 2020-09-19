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
    public class AddParticipantToCalendarItem
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<Participant> _cosmosRepository;
        private CosmosDBRepository<CalendarItem> _calendarRepository;
        public AddParticipantToCalendarItem(ILogger<AddParticipantToCalendarItem> logger, 
                                            ServerSettingsRepository serverSettingsRepository, 
                                            CosmosDBRepository<Participant> cosmosRepository,
                                            CosmosDBRepository<CalendarItem> calendarRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosRepository = cosmosRepository;
            _calendarRepository = calendarRepository;
        }

        [FunctionName("AddParticipantToCalendarItem")]
        [OpenApiOperation(Summary = "Add a participant to the referenced CalendarItem.",
                          Description = "If the Participants already exists (same id) it is overwritten.")]
        [OpenApiRequestBody("application/json", typeof(Participant), Description = "New Participant to be written.")]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(BackendResult), Description = "Status of operation.")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation($"C# HTTP trigger function AddParticipantToCalendarItem processed a request.");
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
            // Get and check corresponding CalendarItem
            if (String.IsNullOrEmpty(participant.CalendarItemId))
            {
                return new OkObjectResult(new BackendResult(false, "Terminangabe fehlt."));
            }
            CalendarItem calendarItem = await _calendarRepository.GetItem(participant.CalendarItemId);
            if (null == calendarItem)
            {
                return new OkObjectResult(new BackendResult(false, "Angegebenen Termin nicht gefunden."));
            }
            // Get participant list to check max registrations and if caller is already registered.
            IEnumerable<Participant> participants = await _cosmosRepository.GetItems(p => p.CalendarItemId.Equals(calendarItem.Id));
            int counter = calendarItem.WithoutHost ? 0 : 1;
            foreach (Participant p in participants)
            {
                if (p.ParticipantFirstName.Equals(participant.ParticipantFirstName) && p.ParticipantLastName.Equals(participant.ParticipantLastName))
                {
                    return new OkObjectResult(new BackendResult(false, "Bereits registriert."));
                }
                ++counter;
            }
            int maxRegistrationCount = calendarItem.MaxRegistrationsCount;
            if (serverSettings.IsAdmin(keyWord))
            {
                // Admin can "overbook" a meetup to be able to add some extra guests
                maxRegistrationCount *= Constants.ADMINOVERBOOKFACTOR;
            }
            if (counter >= maxRegistrationCount)
            {
                return new OkObjectResult(new BackendResult(false, "Maximale Anzahl Registrierungen bereits erreicht."));
            }
            // Set TTL for participant the same as for CalendarItem
            System.TimeSpan diffTime = calendarItem.StartDate.Subtract(DateTime.Now);
            participant.TimeToLive = serverSettings.AutoDeleteAfterDays * 24 * 3600 + (int)diffTime.TotalSeconds;
            // Checkindate to track bookings
            participant.CheckInDate = DateTime.Now;
            if(null != tenant)
            { 
                participant.Tenant = tenant;
            }

            participant = await _cosmosRepository.UpsertItem(participant);
            BackendResult result = new BackendResult(true);

            return new OkObjectResult(result);


        }
    }
}
