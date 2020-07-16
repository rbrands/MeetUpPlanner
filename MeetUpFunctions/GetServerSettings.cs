using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using MeetUpPlanner.Shared;
using System.Web.Http;

namespace MeetUpPlanner.Functions
{
    public class GetServerSettings
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private ServerSettingsRepository _cosmosRepository;

        public GetServerSettings(ILogger<GetServerSettings> logger, IConfiguration config, ServerSettingsRepository cosmosRepository)
        {
            _logger = logger;
            _config = config;
            _cosmosRepository = cosmosRepository;
        }
        [FunctionName("GetServerSettings")]
        public  async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function GetServerSettings processed a request.");
            ServerSettings serverSettings = await _cosmosRepository.GetServerSettings();
            string keyWord = req.Headers[Constants.HEADER_KEYWORD];
            if (String.IsNullOrEmpty(keyWord) || !serverSettings.AdminKeyword.Equals(keyWord))
            {
                return new BadRequestErrorMessageResult("Keyword is missing or wrong");
            }
            return new OkObjectResult(serverSettings);
        }
    }
}
