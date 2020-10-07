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
    public class AssignNewHostToCalendarItem
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<Participant> _cosmosRepository;
        private CosmosDBRepository<CalendarItem> _calendarRepository;
        public AssignNewHostToCalendarItem(ILogger<AssignNewHostToCalendarItem> logger,
                                            ServerSettingsRepository serverSettingsRepository,
                                            CosmosDBRepository<Participant> cosmosRepository,
                                            CosmosDBRepository<CalendarItem> calendarRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosRepository = cosmosRepository;
            _calendarRepository = calendarRepository;
        }

        [FunctionName("AssignNewHostToCalendarItem")]
        [OpenApiOperation(Summary = "Assign a new host.",
                          Description = "The given participant will be assigned as new host for the CalendarItem.")]
        [OpenApiRequestBody("application/json", typeof(Participant), Description = "Participant to be the new host.")]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(BackendResult), Description = "Status of operation.")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation($"C# HTTP trigger function AssignNewHostToCalendarItem processed a request.");
            string tenant = req.Headers[Constants.HEADER_TENANT];
            if (String.IsNullOrWhiteSpace(tenant))
            {
                tenant = null;
            }
            ServerSettings serverSettings = await _serverSettingsRepository.GetServerSettings(tenant);

            string keyWord = req.Headers[Constants.HEADER_KEYWORD];
            if (String.IsNullOrEmpty(keyWord) || !(serverSettings.IsUser(keyWord)))
            {
                return new BadRequestErrorMessageResult("Keyword is missing or wrong.");
            }
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Participant participant = JsonConvert.DeserializeObject<Participant>(requestBody);
            // Check if participant has an id
            if (String.IsNullOrEmpty(participant.Id))
            {
                return new OkObjectResult(new BackendResult(false, "Id des Teilnehmers fehlt."));
            }
            if (String.IsNullOrEmpty(participant.ParticipantFirstName) || String.IsNullOrEmpty(participant.ParticipantLastName))
            {
                return new OkObjectResult(new BackendResult(false, "Name unvollständig."));
            }
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
            // Assign participant as new host
            calendarItem.HostFirstName = participant.ParticipantFirstName;
            calendarItem.HostLastName = participant.ParticipantLastName;
            calendarItem.HostAdressInfo = participant.ParticipantAdressInfo;
            calendarItem.WithoutHost = false;
            // Set TTL and write CalendarItem to database
            System.TimeSpan diffTime = calendarItem.StartDate.Subtract(DateTime.Now);
            calendarItem.TimeToLive = serverSettings.AutoDeleteAfterDays * 24 * 3600 + (int)diffTime.TotalSeconds;
            calendarItem = await _calendarRepository.UpsertItem(calendarItem);
            // Delete host as participant
            await _cosmosRepository.DeleteItemAsync(participant.Id);
            BackendResult result = new BackendResult(true);

            return new OkObjectResult(result);


        }
    }
}
