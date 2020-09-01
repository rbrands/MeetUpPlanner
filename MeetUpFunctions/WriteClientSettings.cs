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
    public class WriteClientSettings
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<ClientSettings> _cosmosRepository;

        public WriteClientSettings(ILogger<WriteClientSettings> logger, ServerSettingsRepository serverSettingsRepository, CosmosDBRepository<ClientSettings> cosmosRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosRepository = cosmosRepository;
        }

        /// <summary>
        /// Writes a new version of ClientSettings to the database. 
        /// Header "x-meetup-keyword" with the valid administrator keyword required.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName(nameof(WriteClientSettings))]
        [OpenApiOperation(Summary = "Writes new ClientSettings to database.",
                          Description = "Only needed if the ClientSettings has been changed. To be able to write ClientSettings the admin keyword must be set as header x-meetup-keyword.")]
        [OpenApiRequestBody("application/json", typeof(ServerSettings), Description = "New ClientSettings to be written.")]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(ServerSettings), Description = "New ClientSettings as written to database.")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function WriteClientSettings processed a request.");
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
            if (String.IsNullOrEmpty(keyWord) || !serverSettings.AdminKeyword.Equals(keyWord))
            {
                return new BadRequestErrorMessageResult("Keyword is missing or wrong.");
            }
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            ClientSettings clientSettings = JsonConvert.DeserializeObject<ClientSettings>(requestBody);
            clientSettings.LogicalKey = Constants.KEY_CLIENT_SETTINGS;
            if (null != tenant)
            {
                clientSettings.LogicalKey += "-" + tenant;
                clientSettings.Tenant = tenant;
            }
            clientSettings = await _cosmosRepository.UpsertItem(clientSettings);

            return new OkObjectResult(clientSettings);
        }
    }
}
