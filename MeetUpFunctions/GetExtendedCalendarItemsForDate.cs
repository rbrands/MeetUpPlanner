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
    public class GetExtendedCalendarItemsForDate
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<CalendarItem> _cosmosRepository;
        private CosmosDBRepository<Participant> _participantRepository;
        private CosmosDBRepository<CalendarComment> _commentRepository;

        public GetExtendedCalendarItemsForDate(ILogger<GetExtendedCalendarItems> logger, ServerSettingsRepository serverSettingsRepository,
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

        [FunctionName("GetExtendedCalendarItemsForDate")]
        [OpenApiOperation(Summary = "Gets the ExtendedCalendarIitems for the given date",
                          Description = "Reading current ExtendedCalendarItems (CalendarItem including correpondent participants and comments) for the given date. To be able to read CalenderItems the user keyword must be set as header x-meetup-keyword.")]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(IEnumerable<ExtendedCalendarItem>))]
        [OpenApiParameter("privatekeywords", Description = "Holds a list of private keywords, separated by ;")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function GetExtendedCalendarItemsForDate processed a request.");
            string tenant = req.Headers[Constants.HEADER_TENANT];
            if (String.IsNullOrWhiteSpace(tenant))
            {
                tenant = null;
            }
            ServerSettings serverSettings = await _serverSettingsRepository.GetServerSettings(tenant);
            string keyWord = req.Headers[Constants.HEADER_KEYWORD];
            if (String.IsNullOrEmpty(keyWord) || !serverSettings.IsUser(keyWord))
            {
                _logger.LogWarning("GetExtendedCalendarItemsForDate called with wrong keyword.");
                return new BadRequestErrorMessageResult("Keyword is missing or wrong.");
            }
            string privateKeywordsString = req.Query["privatekeywords"];
            string[] privateKeywords = null;
            if (!String.IsNullOrEmpty(privateKeywordsString))
            {
                privateKeywords = privateKeywordsString.Split(';');
            }
            string requestedDateArg = req.Query["requesteddate"];
            if (String.IsNullOrEmpty(requestedDateArg))
            {
                _logger.LogWarning("GetExtendedCalendarItemsForDate called without requesteddate.");
                return new BadRequestErrorMessageResult("requesteddate is misssing..");
            }
            DateTime compareDate = DateTime.Parse(requestedDateArg);

            // Get a list of all CalendarItems and filter all applicable ones
            IEnumerable<CalendarItem> rawListOfCalendarItems;
            if (null == tenant)
            {
                rawListOfCalendarItems = await _cosmosRepository.GetItems(d => d.StartDate > compareDate && d.StartDate < compareDate.AddHours(24.0) && (d.Tenant ?? String.Empty) == String.Empty);
            }
            else
            {
                rawListOfCalendarItems = await _cosmosRepository.GetItems(d => d.StartDate > compareDate && d.StartDate < compareDate.AddHours(24.0) && d.Tenant.Equals(tenant));
            }
            List<ExtendedCalendarItem> resultCalendarItems = new List<ExtendedCalendarItem>(10);
            foreach (CalendarItem item in rawListOfCalendarItems)
            {
                // Create ExtendedCalendarItem and get comments and participants
                ExtendedCalendarItem extendedItem = new ExtendedCalendarItem(item);
                if (!serverSettings.IsAdmin(keyWord) && extendedItem.PublishDate.CompareTo(DateTime.UtcNow) > 0)
                {
                    // If calendar item is not ready for publishing skip it
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
                extendedItem.CommentsList = await _commentRepository.GetItems(c => c.CalendarItemId.Equals(extendedItem.Id));
            }
            IEnumerable<ExtendedCalendarItem> orderedList = resultCalendarItems.OrderBy(d => d.StartDate);
            return new OkObjectResult(orderedList);
        }
    }
}
