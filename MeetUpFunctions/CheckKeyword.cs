using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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

        /// <summary>
        /// Checks the given keyword and returns the status: IsUser and IsAdmin
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName(nameof(CheckKeyword))]
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
                keywordCheck.IsParticipant = _serverSettingsRepository.IsInvitedGuest(keyword);
                keywordCheck.IsUser = keyword.Equals(serverSettings.UserKeyword);
                keywordCheck.IsAdmin = keyword.Equals(serverSettings.AdminKeyword);
            }
            if (keywordCheck.IsUser || keywordCheck.IsAdmin)
            {
                // Users and Admins are also participants
                keywordCheck.IsParticipant = true;
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
