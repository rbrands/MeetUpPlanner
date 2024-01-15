using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MeetUpPlanner.Shared;
using System.Web.Http;


namespace MeetUpPlanner.Functions
{
    public class GetContentWithChaptersItem
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<ContentWithChaptersItem> _cosmosRepository;

        public GetContentWithChaptersItem(ILogger<GetContentWithChaptersItem> logger, ServerSettingsRepository serverSettingsRepository, CosmosDBRepository<ContentWithChaptersItem> cosmosRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosRepository = cosmosRepository;
        }

        [FunctionName("GetContentWithChaptersItem")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetContentWithChaptersItem/{key}")] HttpRequest req,
            string key)
        {
            try
            {
                _logger.LogInformation($"GetContentWithChaptersItem(key = {key})");
                string tenant = null;
                if (req.Headers.ContainsKey(Constants.HEADER_TENANT))
                {
                    tenant = req.Headers[Constants.HEADER_TENANT];
                }
                if (String.IsNullOrWhiteSpace(tenant))
                {
                    tenant = null;
                }
                if (null != tenant)
                {
                    key += "-" + tenant;
                }
                ServerSettings serverSettings = await _serverSettingsRepository.GetServerSettings(tenant);
                ContentWithChaptersItem item = await _cosmosRepository.GetItemByKey(key);
                return new OkObjectResult(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetContentWithChaptersItem() failed.");
                return new BadRequestErrorMessageResult(ex.Message);
            }
        }
    }
}
