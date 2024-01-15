using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using MeetUpPlanner.Shared;
using System.Web.Http;
using System.Linq;

namespace MeetUpPlanner.Functions
{
    public class GetExportLog
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<ExportLogItem> _cosmosRepository;

        public GetExportLog(ILogger<GetExportLog> logger, ServerSettingsRepository serverSettingsRepository, CosmosDBRepository<ExportLogItem> cosmosRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosRepository = cosmosRepository;
        }

        [FunctionName("GetExportLog")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function GetExportLog processed a request.");
            string tenant = req.Headers[Constants.HEADER_TENANT];
            if (String.IsNullOrWhiteSpace(tenant))
            {
                tenant = null;
            }
            ServerSettings serverSettings = await _serverSettingsRepository.GetServerSettings(tenant);

            string keyWord = req.Headers[Constants.HEADER_KEYWORD];
            if (String.IsNullOrEmpty(keyWord) || !serverSettings.IsAdmin(keyWord))
            {
                _logger.LogWarning("GetExportLog called with wrong keyword.");
                return new BadRequestErrorMessageResult("Keyword is missing or wrong.");
            }


            IEnumerable<ExportLogItem> exportLog;
            if (null == tenant)
            { 
                exportLog = await _cosmosRepository.GetItems(l => (l.Tenant ?? String.Empty) == String.Empty);
            }
            else
            {
                exportLog = await _cosmosRepository.GetItems(l => l.Tenant.Equals(tenant));
            }
            IEnumerable<ExportLogItem> orderedList = exportLog.OrderByDescending(l => l.RequestDate);
            return new OkObjectResult(orderedList);
        }
    }
}
