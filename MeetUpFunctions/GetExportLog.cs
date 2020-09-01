using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using Newtonsoft.Json;
using MeetUpPlanner.Shared;
using System.Web.Http;
using System.Linq;
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;

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
        [OpenApiOperation(Summary = "Returns all export log items",
                          Description = "Reading all ExportLogItems. To be able to read ExportLogItems the admin keyword must be set as header x-meetup-keyword.")]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(IEnumerable<ExportLogItem>))]
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
