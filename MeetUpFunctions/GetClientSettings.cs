using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MeetUpPlanner.Shared;
using Microsoft.Extensions.Configuration;
using MeetUpPlanner.Functions;

namespace MeetUpFunctions
{
    public class GetClientSettings
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private CosmosDBRepository<ClientSettings> _cosmosRepository;

        public GetClientSettings(ILogger<GetClientSettings> logger, IConfiguration config, CosmosDBRepository<ClientSettings> cosmosRepository)
        {
            _logger = logger;
            _config = config;
            _cosmosRepository = cosmosRepository;
        }

        [FunctionName(nameof(GetClientSettings))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function GetClientSettings processed a request.");

            ClientSettings clientSettings = await _cosmosRepository.GetItemByKey(Constants.KEY_CLIENT_SETTINGS);
            if (null == clientSettings)
            {
                clientSettings = new ClientSettings()
                {
                    Title = Constants.DEFAULT_TITLE,
                    FurtherInfoLink = Constants.DEFAULT_LINK,
                    FurtherInfoTitle = Constants.DEFAULT_LINK_TITLE
                };
            }
            return new OkObjectResult(clientSettings);
        }
    }
}
