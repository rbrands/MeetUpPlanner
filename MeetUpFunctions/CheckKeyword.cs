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
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;


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

        /// <summary>
        /// Checks the given keyword and returns the status: IsUser and IsAdmin
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName(nameof(CheckKeyword))]
        [OpenApiOperation(Summary = "Checks the given keyword and returns if it matches the user or admin keyword.",
                          Description = "Reading the ServerSettings is only needed for editing for administrators. To be able to read ServerSettings the admin keyword must be set as header x-meetup-keyword.")]
        [OpenApiParameter("keyword", Description = "Keyword to be checked.")]
        [OpenApiResponseBody(System.Net.HttpStatusCode.OK, "application/json", typeof(KeywordCheck), Description = "With status IsUser and IsAdmin.")]
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
