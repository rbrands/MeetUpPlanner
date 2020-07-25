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
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;
using MeetUpPlanner.Shared;


namespace MeetUpPlanner.Functions
{
    public class AddParticipantToCalendarItem
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<Participant> _cosmosRepository;
        public AddParticipantToCalendarItem(ILogger<WriteCalendarItem> logger, ServerSettingsRepository serverSettingsRepository, CosmosDBRepository<Participant> cosmosRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosRepository = cosmosRepository;
        }

        [FunctionName("AddParticipantToCalendarItem")]
        [OpenApiOperation(Summary = "Add a participant to the referenced CalendarItem.",
                          Description = "If the Participants already exists (same id) it it overwritten.")]
        [OpenApiRequestBody("application/json", typeof(Participant), Description = "New Participant to be written.")]
        [OpenApiResponseBody(System.Net.HttpStatusCode.OK, "application/json", typeof(Participant), Description = "New Participant as written to database.")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation($"C# HTTP trigger function AddParticipantToCalendarItem processed a request.");
            ServerSettings serverSettings = await _serverSettingsRepository.GetServerSettings();

            string keyWord = req.Headers[Constants.HEADER_KEYWORD];
            if (String.IsNullOrEmpty(keyWord) || !serverSettings.IsUser(keyWord))
            {
                return new BadRequestErrorMessageResult("Keyword is missing or wrong.");
            }
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Participant participant = JsonConvert.DeserializeObject<Participant>(requestBody);
            participant.CheckInDate = DateTime.Now;
            participant = await _cosmosRepository.UpsertItem(participant);

            return new OkObjectResult(participant);


        }
    }
}
