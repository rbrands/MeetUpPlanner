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
    public class WriteServerSettings
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        public WriteServerSettings(ILogger<WriteServerSettings> logger, ServerSettingsRepository serverSettingsRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
        }

        [FunctionName("WriteServerSettings")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function WriteServerSettings processed a request.");

            ServerSettings serverSettings = await _serverSettingsRepository.GetServerSettings();
            string keyWord = req.Headers[Constants.HEADER_KEYWORD];
            if (String.IsNullOrEmpty(keyWord) || !serverSettings.AdminKeyword.Equals(keyWord))
            {
                return new BadRequestErrorMessageResult("Keyword is missing or wrong");
            }
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            ServerSettings newServerSettings = JsonConvert.DeserializeObject<ServerSettings>(requestBody);
            newServerSettings = await _serverSettingsRepository.WriteServerSettings(newServerSettings);

            return new OkObjectResult(newServerSettings);
        }
    }
}
