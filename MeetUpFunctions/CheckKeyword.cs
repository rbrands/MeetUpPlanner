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

namespace MeetUpPlanner.Functions
{
    public class CheckKeyword
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;

        public CheckKeyword(ILogger<CheckKeyword> logger, ServerSettingsRepository serverSettingsRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
        }


        [FunctionName(nameof(CheckKeyword))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger CheckKeyword function processed a request.");
            ServerSettings serverSettings = await _serverSettingsRepository.GetServerSettings();
            string keyword = req.Query["keyword"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            keyword = keyword ?? data?.keyword;

            KeywordCheck keywordCheck = new KeywordCheck();
            if (!String.IsNullOrEmpty(keyword))
            {
                keywordCheck.IsUser = keyword.Equals(serverSettings.UserKeyword);
                keywordCheck.IsAdmin = keyword.Equals(serverSettings.AdminKeyword);
            }
            if (keywordCheck.IsAdmin)
            {
                // Admin is user, too
                keywordCheck.IsUser = true;
            }

            return new OkObjectResult(keywordCheck);
        }
    }
}
