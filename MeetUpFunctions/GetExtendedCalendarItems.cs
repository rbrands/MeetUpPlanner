using System;
using System.IO;
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
    public class GetExtendedCalendarItems
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<CalendarItem> _cosmosRepository;
        private CosmosDBRepository<Participant> _participantRepository;
        private CosmosDBRepository<CalendarComment> _commentRepository;

        public GetExtendedCalendarItems(ILogger<GetExtendedCalendarItems> logger, ServerSettingsRepository serverSettingsRepository, 
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

        [FunctionName("GetExtendedCalendarItems")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            string tenant = req.Headers[Constants.HEADER_TENANT];
            if (String.IsNullOrWhiteSpace(tenant))
            {
                tenant = null;
            }
            string tenantBadge = null == tenant ? "default" : tenant;
            _logger.LogInformation($"GetExtendedCalendarItems for tenant <{tenantBadge}>");
            ServerSettings serverSettings = await _serverSettingsRepository.GetServerSettings(tenant);
            string keyWord = req.Headers[Constants.HEADER_KEYWORD];
            if (String.IsNullOrEmpty(keyWord) || !(serverSettings.IsUser(keyWord) || _serverSettingsRepository.IsInvitedGuest(keyWord) || serverSettings.IsWebcalToken(keyWord)))
            {
                _logger.LogWarning($"GetExtendedCalendarItems<{tenantBadge}> called with wrong keyword.");
                return new BadRequestErrorMessageResult("Keyword is missing or wrong.");
            }
            string privateKeywordsString = req.Query["privatekeywords"];
            string[] privateKeywords = null;
            if (!String.IsNullOrEmpty(privateKeywordsString))
            {
                privateKeywords = privateKeywordsString.Split(';');
            }

            // Get a list of all CalendarItems and filter all applicable ones
            DateTime compareDate = DateTime.Now.AddHours((-serverSettings.CalendarItemsPastWindowHours));

            IEnumerable<CalendarItem> rawListOfCalendarItems;
            if (null == tenant)
            {
                rawListOfCalendarItems = await _cosmosRepository.GetItems(d => d.StartDate > compareDate && (d.Tenant ?? String.Empty) == String.Empty);
            }
            else
            {
                rawListOfCalendarItems = await _cosmosRepository.GetItems(d => d.StartDate > compareDate && d.Tenant.Equals(tenant));
            }
            IEnumerable<CalendarItem> rawCalendarItemsWithFederatedOnes = rawListOfCalendarItems;
            if (!String.IsNullOrEmpty(serverSettings.Federation))
            {
                IEnumerable<CalendarItem> rawListOfFederatedCalendarItems;
                rawListOfFederatedCalendarItems = await _cosmosRepository.GetItems(d => d.StartDate > compareDate && d.Federation.Equals(serverSettings.Federation));
                rawCalendarItemsWithFederatedOnes = rawListOfCalendarItems.Concat(rawListOfFederatedCalendarItems);
            }
            List<ExtendedCalendarItem> resultCalendarItems = new List<ExtendedCalendarItem>(10);
            foreach (CalendarItem item in rawCalendarItemsWithFederatedOnes)
            {
                // Create ExtendedCalendarItem and get comments and participants
                ExtendedCalendarItem extendedItem = new ExtendedCalendarItem(item);
                if (!serverSettings.IsAdmin(keyWord) && extendedItem.PublishDate.CompareTo(DateTime.UtcNow) > 0)
                {
                    // If calendar item is not ready for publishing skip it
                    continue;
                }
                if (item.IsInternal && !serverSettings.IsUser(keyWord) && !serverSettings.IsWebcalToken(keyWord))
                {
                    // If calendar item is only internal and user is not a regular one (with proper keyword) skip it
                    continue;
                }
                if (String.IsNullOrEmpty(extendedItem.PrivateKeyword))
                {
                    // No private keyword for item ==> use it
                    resultCalendarItems.Add(extendedItem);
                }
                else if (null != privateKeywords)
                {
                    // Private keyword for item ==> check given keyword list against it
                    foreach (string keyword in privateKeywords)
                    {
                        if (keyword.Equals(extendedItem.PrivateKeyword))
                        {
                            resultCalendarItems.Add(extendedItem);
                            break;
                        }
                    }
                }
                // Read all participants for this calendar item
                extendedItem.ParticipantsList = await _participantRepository.GetItems(p => p.CalendarItemId.Equals(extendedItem.Id));
                // Read all comments
                extendedItem.CommentsList = (await _commentRepository.GetItems(c => c.CalendarItemId.Equals(extendedItem.Id))).OrderByDescending(c => c.CommentDate);
            }
            IEnumerable<ExtendedCalendarItem> orderedList = resultCalendarItems.OrderBy(d => d.StartDate);
            return new OkObjectResult(orderedList);
        }
    }
}
