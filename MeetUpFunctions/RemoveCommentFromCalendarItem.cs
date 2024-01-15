using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Web.Http;
using MeetUpPlanner.Shared;

namespace MeetUpPlanner.Functions
{
    public class RemoveCommentFromCalendarItem
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<CalendarComment> _cosmosRepository;
        public RemoveCommentFromCalendarItem(ILogger<RemoveCommentFromCalendarItem> logger,
                                             ServerSettingsRepository serverSettingsRepository,
                                             CosmosDBRepository<CalendarComment> cosmosRepository
                                            )
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosRepository = cosmosRepository;
        }

        [FunctionName("RemoveCommentFromCalendarItem")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation($"C# HTTP trigger function RemoveCommentFromCalendarItem processed a request.");
            string tenant = req.Headers[Constants.HEADER_TENANT];
            if (String.IsNullOrWhiteSpace(tenant))
            {
                tenant = null;
            }
            ServerSettings serverSettings = await _serverSettingsRepository.GetServerSettings(tenant);
            string keyWord = req.Headers[Constants.HEADER_KEYWORD];
            if (String.IsNullOrEmpty(keyWord) || !serverSettings.IsUser(keyWord))
            {
                return new BadRequestErrorMessageResult("Keyword is missing or wrong.");
            }
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            CalendarComment comment = JsonConvert.DeserializeObject<CalendarComment>(requestBody);
            if (String.IsNullOrEmpty(comment.Id))
            {
                return new OkObjectResult(new BackendResult(false, "Die Id des Kommentars fehlt."));
            }
            await _cosmosRepository.DeleteItemAsync(comment.Id);

            return new OkObjectResult(new BackendResult(true));
        }
    }
}

