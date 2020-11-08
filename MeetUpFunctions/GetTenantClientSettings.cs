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
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;
using System.Web.Http;


namespace MeetUpPlanner.Functions
{
    public class GetTenantClientSettings
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _config;
        private CosmosDBRepository<MeetUpPlanner.Shared.TenantSettings> _tenantRepository;
        private CosmosDBRepository<ClientSettings> _cosmosRepository;

        public GetTenantClientSettings(ILogger<GetTenantClientSettings> logger, 
                                       IConfiguration config,
                                       CosmosDBRepository<MeetUpPlanner.Shared.TenantSettings> tenantRepository,
                                       CosmosDBRepository<ClientSettings> cosmosRepository)
        {
            _logger = logger;
            _config = config;
            _tenantRepository = tenantRepository;
            _cosmosRepository = cosmosRepository;
        }

        /// <summary>
        /// Get the current ClientSettings
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName(nameof(GetTenantClientSettings))]
        [OpenApiOperation(Summary = "Gets the active combination of TenantSettings/ClientSettings", Description = "Reading TenantSettings and ClientSettings should be done at the very beginning of the client application.")]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(ClientSettings))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            string tenantUrl = req.Headers[Constants.HEADER_TENANT_URL];
            if (String.IsNullOrEmpty(tenantUrl) )
            {
                return new BadRequestErrorMessageResult($"GetTenantClientSettings: Header {Constants.HEADER_TENANT_URL}  is missing or wrong");
            }
            _logger.LogInformation($"GetTenantClientSettings: {Constants.HEADER_TENANT_URL} = {tenantUrl}");
            TenantClientSettings tenantClientSettings = new TenantClientSettings();
           
            // First try to get tenant settings from database
            tenantClientSettings.Tenant = await _tenantRepository.GetFirstItemOrDefault(t => tenantUrl.StartsWith(t.PrimaryUrl));
            if (null != tenantClientSettings.Tenant)
            {
                _logger.LogInformation($"GetTenantClientSetting({tenantUrl}): Tenant Settings {tenantClientSettings.Tenant.TenantName} found.");
            }
            else
            {
                // If not in database get from static configuration table. This allows that MeetUpPlanner can run withour admin tool
                tenantClientSettings.Tenant = TenantConfig.GetTenant(tenantUrl);
            }

            string key = Constants.KEY_CLIENT_SETTINGS;
            string tenant = tenantClientSettings.Tenant.TenantKey;
            if (null != tenant)
            {
                key += "-" + tenant;
            }
            tenantClientSettings.Client = await _cosmosRepository.GetItemByKey(key);
            if (null == tenantClientSettings.Client)
            {
                tenantClientSettings.Client = new ClientSettings()
                {
                    Title = Constants.DEFAULT_TITLE,
                    FurtherInfoLink = Constants.DEFAULT_LINK,
                    FurtherInfoTitle = Constants.DEFAULT_LINK_TITLE,
                    Disclaimer = Constants.DEFAULT_DISCLAIMER,
                    GuestDisclaimer = Constants.DEFAULT_GUEST_DISCLAIMER
                };
            }
            return new OkObjectResult(tenantClientSettings);
        }
    }
}
