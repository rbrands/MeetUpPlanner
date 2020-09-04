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
    public class GetAllExtendedCalendarItems
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<CalendarItem> _cosmosRepository;
        private CosmosDBRepository<Participant> _participantRepository;
        private CosmosDBRepository<CalendarComment> _commentRepository;

        public GetAllExtendedCalendarItems(ILogger<GetExtendedCalendarItems> logger, ServerSettingsRepository serverSettingsRepository,
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

        [FunctionName("GetAllExtendedCalendarItems")]
        [OpenApiOperation(Summary = "Gets all ExtendedCalendarIitems",
                          Description = "Reading all ExtendedCalendarItems (including particpants and comments) for tracking issues. To be able to read all ExtendedCalenderItems the admin keyword must be set as header x-meetup-keyword.")]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(IEnumerable<ExtendedCalendarItem>))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function GetAllExtendedCalendarItems processed a request.");
            string tenant = req.Headers[Constants.HEADER_TENANT];
            ServerSettings serverSettings = await _serverSettingsRepository.GetServerSettings(tenant);

            string keyWord = req.Headers[Constants.HEADER_KEYWORD];
            if (String.IsNullOrEmpty(keyWord) || !serverSettings.IsAdmin(keyWord))
            {
                _logger.LogWarning("GetAllExtendedCalendarItems called with wrong keyword.");
                return new BadRequestErrorMessageResult("Keyword is missing or wrong.");
            }
            // Get a list of all CalendarItems

            IEnumerable<CalendarItem> rawListOfCalendarItems = await _cosmosRepository.GetItems();
            List<ExtendedCalendarItem> resultCalendarItems = new List<ExtendedCalendarItem>(50);
            foreach (CalendarItem item in rawListOfCalendarItems)
            {
                ExtendedCalendarItem extendedItem = new ExtendedCalendarItem(item);
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
