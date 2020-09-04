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
    public class GetExtendedCalendarItem
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<CalendarItem> _cosmosRepository;
        private CosmosDBRepository<Participant> _participantRepository;
        private CosmosDBRepository<CalendarComment> _commentRepository;

        public GetExtendedCalendarItem(ILogger<GetExtendedCalendarItem> logger, 
                                       ServerSettingsRepository serverSettingsRepository,
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

        [FunctionName("GetExtendedCalendarItem")]
        [OpenApiOperation(Summary = "Gets the CalendarIitem with all referencing data",
                          Description = "Reading given CalendarItem To be able to read CalenderItems the user keyword must be set as header x-meetup-keyword.")]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(ExtendedCalendarItem))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetExtendedCalendarItem/{id}")] HttpRequest req, string id)
        {
            _logger.LogInformation("C# HTTP trigger function GetExtendedCalendarItem processed a request.");
            string tenant = req.Headers[Constants.HEADER_TENANT];
            if (String.IsNullOrWhiteSpace(tenant))
            {
                tenant = null;
            }
            ServerSettings serverSettings = await _serverSettingsRepository.GetServerSettings(tenant);
            string keyWord = req.Headers[Constants.HEADER_KEYWORD];
            if (String.IsNullOrEmpty(keyWord) || !(serverSettings.IsUser(keyWord) || _serverSettingsRepository.IsInvitedGuest(keyWord)))
            {
                _logger.LogWarning("GetExtendedCalendarItem called with wrong keyword.");
                return new BadRequestErrorMessageResult("Keyword is missing or wrong.");
            }
            // Get CalendarItem by id
            CalendarItem rawCalendarItem = await _cosmosRepository.GetItem(id);
            if (null == rawCalendarItem)
            {
                return new BadRequestErrorMessageResult("No CalendarItem with given id found.");
            }
            ExtendedCalendarItem extendedItem = new ExtendedCalendarItem(rawCalendarItem);
            // Read all participants for this calendar item
            extendedItem.ParticipantsList = await _participantRepository.GetItems(p => p.CalendarItemId.Equals(extendedItem.Id));
            // Read all comments
            extendedItem.CommentsList = await _commentRepository.GetItems(c => c.CalendarItemId.Equals(extendedItem.Id));
            return new OkObjectResult(extendedItem);
        }
    }
}
