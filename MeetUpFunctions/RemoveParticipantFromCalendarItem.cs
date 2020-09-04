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
        public RemoveParticipantFromCalendarItem(ILogger<RemoveParticipantFromCalendarItem> logger,
                                            ServerSettingsRepository serverSettingsRepository,
                                            CosmosDBRepository<Participant> cosmosRepository
                                            )
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosRepository = cosmosRepository;
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
            await _cosmosRepository.DeleteItemAsync(participant.Id);

            return new OkObjectResult(new BackendResult(true));
        }
    }
}

