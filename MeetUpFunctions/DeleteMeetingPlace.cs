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
    public class DeleteMeetingPlace
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<MeetingPlace> _cosmosRepository;
        public DeleteMeetingPlace(ILogger<DeleteInfoItem> logger,
                                  ServerSettingsRepository serverSettingsRepository,
                                  CosmosDBRepository<MeetingPlace> cosmosRepository
                             )
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosRepository = cosmosRepository;
        }

        [FunctionName("DeleteMeetingPlace")]
        [OpenApiOperation(Summary = "Deletes a MeetingPlace.",
                          Description = "Every MeetingPlace has a unique id that is used to delete it.")]
        [OpenApiRequestBody("application/json", typeof(InfoItem), Description = "MeetingPlace to be removed.")]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(BackendResult), Description = "Status of operation.")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation($"C# HTTP trigger function DeleteMeetingPlace processed a request.");
            string tenant = req.Headers[Constants.HEADER_TENANT];
            if (String.IsNullOrWhiteSpace(tenant))
            {
                tenant = null;
            }
            ServerSettings serverSettings = await _serverSettingsRepository.GetServerSettings(tenant);

            string keyWord = req.Headers[Constants.HEADER_KEYWORD];
            if (String.IsNullOrEmpty(keyWord) || !serverSettings.IsAdmin(keyWord))
            {
                return new BadRequestErrorMessageResult("Keyword is missing or wrong.");
            }
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            MeetingPlace meetingPlace = JsonConvert.DeserializeObject<MeetingPlace>(requestBody);
            if (String.IsNullOrEmpty(meetingPlace.Id))
            {
                return new OkObjectResult(new BackendResult(false, "Die Id des MeetingPlace fehlt."));
            }
            await _cosmosRepository.DeleteItemAsync(meetingPlace.Id);

            return new OkObjectResult(new BackendResult(true));
        }
    }
}

