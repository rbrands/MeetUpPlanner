using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using MeetUpPlanner.Shared;
using System.Web.Http;
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;

namespace MeetUpPlanner.Functions
{
    public class GetServerSettings
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private ServerSettingsRepository _cosmosRepository;
        private CosmosDBRepository<TenantSettings> _tenantRepository;

        public GetServerSettings(ILogger<GetServerSettings> logger, IConfiguration config, ServerSettingsRepository cosmosRepository, CosmosDBRepository<TenantSettings> tenantRepository)
        {
            _logger = logger;
            _config = config;
            _tenantRepository = tenantRepository;
            _cosmosRepository = cosmosRepository;
        }
        /// <summary>
        /// Gets the current version of ServerSettings. Should be used before allow administrators to edit it.
        /// </summary>
        /// <param name="req"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        [FunctionName("GetServerSettings")]
        [OpenApiOperation(Summary = "Gets the active ServerSettings", 
                          Description = "Reading the ServerSettings is only needed for editing for administrators. To be able to read ServerSettings the admin keyword must be set as header x-meetup-keyword.")]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(ServerSettings))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("GetServerSettings");
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
            ServerSettings serverSettings = await _cosmosRepository.GetServerSettings(tenant);
            string keyWord = req.Headers[Constants.HEADER_KEYWORD];
            if (String.IsNullOrEmpty(keyWord) || !serverSettings.AdminKeyword.Equals(keyWord))
            {
                return new BadRequestErrorMessageResult("Keyword is missing or wrong");
            }
            return new OkObjectResult(serverSettings);
        }
    }
}
