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
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;


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
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(KeywordCheck), Description = "With status IsUser and IsAdmin.")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger CheckKeyword function processed a request.");
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
            string keyword = req.Headers[Constants.HEADER_KEYWORD];

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
