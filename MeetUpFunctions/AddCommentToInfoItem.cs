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
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;

namespace MeetUpPlanner.Functions
{
    public class AddCommentToInfoItem
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<CalendarComment> _cosmosRepository;
        private CosmosDBRepository<InfoItem> _infoRepository;
        public AddCommentToInfoItem(ILogger<AddCommentToInfoItem> logger,
                                            ServerSettingsRepository serverSettingsRepository,
                                            CosmosDBRepository<CalendarComment> cosmosRepository,
                                            CosmosDBRepository<InfoItem> infoRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosRepository = cosmosRepository;
            _infoRepository = infoRepository;
        }

        [FunctionName("AddCommentToInfoItem")]
        [OpenApiOperation(Summary = "Add a comment to the referenced InfoItem.",
                          Description = "If the CalendarComment already exists (same id) it is overwritten.")]
        [OpenApiRequestBody("application/json", typeof(CalendarComment), Description = "New CalendarComment to be written.")]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(BackendResult), Description = "Status of operation.")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation($"C# HTTP trigger function AddCommentToInfoItem processed a request.");
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
            // Get and check corresponding CalendarItem
            if (String.IsNullOrEmpty(comment.CalendarItemId))
            {
                return new OkObjectResult(new BackendResult(false, "Terminangabe fehlt."));
            }
            InfoItem infoItem = await _infoRepository.GetItem(comment.CalendarItemId);
            if (null == infoItem)
            {
                return new OkObjectResult(new BackendResult(false, "Angegebenen Termin nicht gefunden."));
            }
            if (!infoItem.CommentsAllowed)
            {
                return new OkObjectResult(new BackendResult(false, "Keine Kommentare hier erlaubt."));
            }
            if (infoItem.CommentsLifeTimeInDays > 0)
            {
                comment.TimeToLive = infoItem.CommentsLifeTimeInDays * 24 * 3600;
            }
            comment.CommentDate = DateTime.Now;
            if (!String.IsNullOrWhiteSpace(tenant))
            {
                comment.Tenant = tenant;
            }
            comment = await _cosmosRepository.UpsertItem(comment);
            BackendResult result = new BackendResult(true);

            return new OkObjectResult(result);
        }
    }
}
