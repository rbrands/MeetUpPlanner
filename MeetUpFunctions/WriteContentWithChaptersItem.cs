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
    public class WriteContentWithChaptersItem
    {
        private readonly ILogger _logger;
        private CosmosDBRepository<ContentWithChaptersItem> _cosmosRepository;
        private ServerSettingsRepository _serverSettingsRepository;

        public WriteContentWithChaptersItem(ILogger<WriteContentWithChaptersItem> logger, ServerSettingsRepository serverSettingsRepository, CosmosDBRepository<ContentWithChaptersItem> cosmosRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosRepository = cosmosRepository;
        }

        /// <summary>
        /// Writes a new or updated CalendarItem to the database. x-meetup-keyword must be set to admin keyword.
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [FunctionName("WriteContentWithChaptersItem")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
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
            if (String.IsNullOrEmpty(keyWord) || !serverSettings.IsAdmin(keyWord))
            {
                return new BadRequestErrorMessageResult("Keyword is missing or wrong.");
            }
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            ContentWithChaptersItem infoItem = JsonConvert.DeserializeObject<ContentWithChaptersItem>(requestBody);
            if (String.IsNullOrEmpty(infoItem.LogicalKey))
            {
                return new BadRequestErrorMessageResult("WriteContentChaptersItem requires a logical key.");
            }
            _logger.LogInformation($"WriteContentChaptersItem({infoItem.LogicalKey})");
            if (infoItem.InfoLifeTimeInDays > 0)
            {
                infoItem.TimeToLive = infoItem.InfoLifeTimeInDays * 24 * 3600;
            }
            if (null != tenant)
            {
                infoItem.Tenant = tenant;
            }
            infoItem = await _cosmosRepository.UpsertItem(infoItem);

            return new OkObjectResult(infoItem);
        }
    }
}
