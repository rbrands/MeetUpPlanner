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

        [FunctionName(nameof(WriteClientSettings))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function WriteClientSettings processed a request.");
            ServerSettings serverSettings = await _serverSettingsRepository.GetServerSettings(); 

            string keyWord = req.Headers[Constants.HEADER_KEYWORD];
            if (String.IsNullOrEmpty(keyWord) || !serverSettings.AdminKeyword.Equals(keyWord))
            {
                return new BadRequestErrorMessageResult("Keyword is missing or wrong.");
            }
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            ClientSettings clientSettings = JsonConvert.DeserializeObject<ClientSettings>(requestBody);
            clientSettings.LogicalKey = Constants.KEY_CLIENT_SETTINGS;
            clientSettings = await _cosmosRepository.UpsertItem(clientSettings);

            return new OkObjectResult(clientSettings);
        }
    }
}
