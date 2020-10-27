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
    public class GetScopedCalendarItems
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<CalendarItem> _cosmosRepository;
        private CosmosDBRepository<Participant> _participantRepository;
        private CosmosDBRepository<CalendarComment> _commentRepository;

        public GetScopedCalendarItems(ILogger<GetScopedCalendarItems> logger, ServerSettingsRepository serverSettingsRepository,
                                                                              CosmosDBRepository<CalendarItem> cosmosRepository,
                                                                              CosmosDBRepository<Participant> participantRepository,
                                                                              CosmosDBRepository<CalendarComment> commentRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosRepository = cosmosRepository;
            _participantRepository = participantRepository;
            _commentRepository = commentRepository;
        }

        [FunctionName("GetScopedCalendarItems")]
        [OpenApiOperation(Summary = "Gets the ExtendedCalendarIitems for the given guest scope",
                          Description = "Reading current ExtendedCalendarItems (CalendarItem including correpondent participants and comments) for the given scope.")]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(IEnumerable<ExtendedCalendarItem>))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            string tenant = req.Headers[Constants.HEADER_TENANT];
            if (String.IsNullOrWhiteSpace(tenant))
            {
                tenant = null;
            }
            string tenantBadge = null == tenant ? "default" : tenant;
            _logger.LogInformation($"GetScopedCalendarItems for tenant <{tenantBadge}>");
            ServerSettings serverSettings = await _serverSettingsRepository.GetServerSettings(tenant);
            string scope = req.Query["scope"];
            string scopeToCompare = scope.ToLowerInvariant();
            if (String.IsNullOrEmpty(scope))
            {
                _logger.LogWarning($"GetScopedCalendarItems <{tenantBadge}> called without scope.");
                return new BadRequestErrorMessageResult("scope is misssing..");
            }
            _logger.LogInformation($"GetScopedCalendarItems<{tenantBadge}>(scope={scope})");

            // Get a list of all CalendarItems and filter all applicable ones
            DateTime compareDate = DateTime.Now.AddHours((-serverSettings.CalendarItemsPastWindowHours));
            IEnumerable<CalendarItem> rawListOfCalendarItems;
            if (null == tenant)
            {
                rawListOfCalendarItems = await _cosmosRepository.GetItems(d => d.StartDate > compareDate && d.GuestScope != null && (d.Tenant ?? String.Empty) == String.Empty);
            }
            else
            {
                rawListOfCalendarItems = await _cosmosRepository.GetItems(d => d.StartDate > compareDate && d.GuestScope != null && d.Tenant.Equals(tenant));
            }
            List<ExtendedCalendarItem> resultCalendarItems = new List<ExtendedCalendarItem>(10);
            foreach (CalendarItem item in rawListOfCalendarItems)
            {
                // Create ExtendedCalendarItem and get comments and participants
                ExtendedCalendarItem extendedItem = new ExtendedCalendarItem(item);
                if (extendedItem.PublishDate.CompareTo(DateTime.UtcNow) > 0)
                {
                    // If calendar item is not ready for publishing skip it
                    continue;
                }
                if (!extendedItem.GuestScope.ToLowerInvariant().Equals(scopeToCompare))
                {
                    continue;
                }
                resultCalendarItems.Add(extendedItem);
                // Read all participants for this calendar item
                extendedItem.ParticipantsList = await _participantRepository.GetItems(p => p.CalendarItemId.Equals(extendedItem.Id));
                // Read all comments
                extendedItem.CommentsList = await _commentRepository.GetItems(c => c.CalendarItemId.Equals(extendedItem.Id));
            }
            IEnumerable<ExtendedCalendarItem> orderedList = resultCalendarItems.OrderBy(d => d.StartDate);
            return new OkObjectResult(orderedList);
        }
    }
}
