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
using Aliencube.AzureFunctions.Extensions.OpenApi.Attributes;


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

        /// <summary>
        /// Writes a new version of ServerSettings to the database. 
        /// Header "x-meetup-keyword" with the valid administrator keyword required.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("WriteServerSettings")]
        [OpenApiOperation(Summary = "Writes new ServerSettings to database.",
                          Description = "Only needed if the ServerSettings has been changed.")]
        [OpenApiRequestBody("application/json", typeof(ServerSettings), Description = "New ServerSettings to be written.")]
        [OpenApiResponseBody(System.Net.HttpStatusCode.OK, "application/json", typeof(ServerSettings), Description = "New ServerSettings as written to database.")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function WriteServerSettings processed a request.");

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
                return new BadRequestErrorMessageResult("Keyword is missing or wrong");
            }
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            ServerSettings newServerSettings = JsonConvert.DeserializeObject<ServerSettings>(requestBody);
            if (null != tenant)
            {
                newServerSettings.Tenant = tenant;
            }
            newServerSettings = await _serverSettingsRepository.WriteServerSettings(newServerSettings);

            return new OkObjectResult(newServerSettings);
        }
    }
}
