using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MeetUpPlanner.Shared;
using System.Web.Http;
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;


namespace MeetUpPlanner.Functions
{
    public class WriteMeetingPlace
    {
        private readonly ILogger _logger;
        private CosmosDBRepository<MeetingPlace> _cosmosRepository;
        private ServerSettingsRepository _serverSettingsRepository;

        public WriteMeetingPlace(ILogger<WriteMeetingPlace> logger, ServerSettingsRepository serverSettingsRepository, CosmosDBRepository<MeetingPlace> cosmosRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosRepository = cosmosRepository;
        }

        /// <summary>
        /// Writes a new or updated CalendarItem to the database. x-meetup-keyword must be set to admin keyword.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("WriteMeetingPlace")]
        [OpenApiOperation(Summary = "Writes a new or updated MeetingPlace to database.",
                          Description = "If the MeetingPlace already exists (same id) it is overwritten.")]
        [OpenApiRequestBody("application/json", typeof(InfoItem), Description = "New MeetingPlace to be written.")]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(InfoItem), Description = "New MeetingPlace as written to database.")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function WriteMeetingPlace processed a request.");
            string tenant = req.Headers[Constants.HEADER_TENANT];
            if (String.IsNullOrWhiteSpace(tenant))
            {
                tenant = null;
            }
            ServerSettings serverSettings;
            if (null == tenant)
            {
                serverSettings = await _serverSettingsRepository.GetServerSettings();
            }
            else
            {
                serverSettings = await _serverSettingsRepository.GetServerSettings(tenant);
            }

            string keyWord = req.Headers[Constants.HEADER_KEYWORD];
            if (String.IsNullOrEmpty(keyWord) || !serverSettings.IsAdmin(keyWord))
            {
                return new BadRequestErrorMessageResult("Keyword is missing or wrong.");
            }
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            MeetingPlace meetingPlace = JsonConvert.DeserializeObject<MeetingPlace>(requestBody);
            if (null != tenant)
            {
                meetingPlace.Tenant = tenant;
            }
            meetingPlace = await _cosmosRepository.UpsertItem(meetingPlace);

            return new OkObjectResult(meetingPlace);
        }
    }
}
