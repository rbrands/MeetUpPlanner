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
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;
using MeetUpPlanner.Shared;
using System.Collections.Generic;

namespace MeetUpPlanner.Functions
{
    public class DeleteInfoItem
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<InfoItem> _cosmosRepository;
        private CosmosDBRepository<CalendarComment> _commentRepository;
        public DeleteInfoItem(ILogger<DeleteInfoItem> logger,
                                  ServerSettingsRepository serverSettingsRepository,
                                  CosmosDBRepository<InfoItem> cosmosRepository,
                                  CosmosDBRepository<CalendarComment> commentRepository
                             )
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosRepository = cosmosRepository;
            _commentRepository = commentRepository;
        }

        [FunctionName("DeleteInfoItem")]
        [OpenApiOperation(Summary = "Deletes an InfoItem including all CalendarComment items.",
                          Description = "Every InfoItem has a unique id that is used to delete it.")]
        [OpenApiRequestBody("application/json", typeof(InfoItem), Description = "InfoItem to be removed.")]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(BackendResult), Description = "Status of operation.")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation($"C# HTTP trigger function DeleteInfoItem processed a request.");
            string tenant = req.Headers[Constants.HEADER_TENANT];
            if (String.IsNullOrWhiteSpace(tenant))
            {
                tenant = null;
            }
            ServerSettings serverSettings = await _serverSettingsRepository.GetServerSettings(tenant);

            string keyWord = req.Headers[Constants.HEADER_KEYWORD];
            if (String.IsNullOrEmpty(keyWord) || !serverSettings.IsAdmin(keyWord))
            {
                return new BadRequestErrorMessageResult("Keyword is missing or wrong.");
            }
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            InfoItem infoItem = JsonConvert.DeserializeObject<InfoItem>(requestBody);
            if (String.IsNullOrEmpty(infoItem.Id))
            {
                return new OkObjectResult(new BackendResult(false, "Die Id des Info-Eintrags fehlt."));
            }
            await _cosmosRepository.DeleteItemAsync(infoItem.Id);
            // Delete all comments
            IEnumerable<CalendarComment> comments = await _commentRepository.GetItems(c => c.CalendarItemId.Equals(infoItem.Id));
            foreach (CalendarComment c in comments)
            {
                await _commentRepository.DeleteItemAsync(c.Id);
            }

            return new OkObjectResult(new BackendResult(true));
        }
    }
}

