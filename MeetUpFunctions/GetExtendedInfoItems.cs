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
    public class GetExtendedInfoItems
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<InfoItem> _cosmosRepository;
        private CosmosDBRepository<CalendarComment> _commentRepository;

        public GetExtendedInfoItems(ILogger<GetExtendedInfoItems> logger, ServerSettingsRepository serverSettingsRepository,
                                                                          CosmosDBRepository<InfoItem> cosmosRepository,
                                                                          CosmosDBRepository<CalendarComment> commentRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosRepository = cosmosRepository;
            _commentRepository = commentRepository;
        }

        [FunctionName("GetExtendedInfoItems")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function GetExtendedInfoItems processed a request.");
            string tenant = req.Headers[Constants.HEADER_TENANT];
            if (String.IsNullOrWhiteSpace(tenant))
            {
                tenant = null;
            }
            ServerSettings serverSettings = await _serverSettingsRepository.GetServerSettings(tenant);
            string keyWord = req.Headers[Constants.HEADER_KEYWORD];
            if (String.IsNullOrEmpty(keyWord) || !(serverSettings.IsUser(keyWord) || _serverSettingsRepository.IsInvitedGuest(keyWord)))
            {
                _logger.LogWarning("GetExtendedInfoItems called with wrong keyword.");
                return new BadRequestErrorMessageResult("Keyword is missing or wrong.");
            }
            IEnumerable<InfoItem> rawListOfInfoItems;
            if (null == tenant)
            {
                rawListOfInfoItems = await _cosmosRepository.GetItems(d => (d.Tenant ?? String.Empty) == String.Empty);
            }
            else
            {
                rawListOfInfoItems = await _cosmosRepository.GetItems(d => d.Tenant.Equals(tenant));
            }
            List<ExtendedInfoItem> resultInfoItems = new List<ExtendedInfoItem>(10);
            foreach (InfoItem item in rawListOfInfoItems)
            {
                // Create ExtendedInfoItem and get comments
                ExtendedInfoItem extendedItem = new ExtendedInfoItem(item);
                if (!serverSettings.IsUser(keyWord) && item.IsInternal)
                {
                    // If info item is only internal and user is not a regular one (with proper keyword) skip it
                    continue;
                }
                resultInfoItems.Add(extendedItem);
                // Read all comments
                extendedItem.CommentsList = (await _commentRepository.GetItems(c => c.CalendarItemId.Equals(extendedItem.Id))).OrderByDescending(c => c.CommentDate);
            }
            IEnumerable<ExtendedInfoItem> orderedList = resultInfoItems.OrderBy(d => d.OrderId);
            return new OkObjectResult(orderedList);
        }
    }
}
