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
    public class WriteServerSettings
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<TenantSettings> _tenantRepository;

        public WriteServerSettings(ILogger<WriteServerSettings> logger, ServerSettingsRepository serverSettingsRepository, CosmosDBRepository<TenantSettings> tenantRepository)
        {
            _logger = logger;
            _tenantRepository = tenantRepository;
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
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(ServerSettings), Description = "New ServerSettings as written to database.")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("WriteServerSettings");

            string tenant = req.Headers[Constants.HEADER_TENANT];
            if (String.IsNullOrWhiteSpace(tenant))
            {
                tenant = null;
            }
            TenantSettings tenantSettings;
            if (null == tenant)
            {
                tenantSettings = await _tenantRepository.GetFirstItemOrDefault(t => t.TenantKey == null);
            }
            else
            {
                tenantSettings = await _tenantRepository.GetFirstItemOrDefault(t => t.TenantKey.Equals(tenant));
            }
            if (tenantSettings.LocalAdministrationDisabled)
            {
                return new BadRequestErrorMessageResult("Administration in MeetUpPlanner is disabled.");
            }

            ServerSettings serverSettings = await _serverSettingsRepository.GetServerSettings(tenant);

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
