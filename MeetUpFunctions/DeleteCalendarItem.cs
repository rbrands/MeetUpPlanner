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
using System.Collections.Generic;

namespace MeetUpPlanner.Functions
{
    public class DeleteCalendarItem
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<CalendarItem> _cosmosRepository;
        private CosmosDBRepository<Participant> _participantRepository;
        private CosmosDBRepository<CalendarComment> _commentRepository;
        public DeleteCalendarItem(ILogger<DeleteCalendarItem> logger,
                                  ServerSettingsRepository serverSettingsRepository,
                                  CosmosDBRepository<CalendarItem> cosmosRepository,
                                  CosmosDBRepository<CalendarComment> commentRepository,
                                  CosmosDBRepository<Participant> participantRepository
                                            )
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosRepository = cosmosRepository;
            _participantRepository = participantRepository;
            _commentRepository = commentRepository;
        }

        [FunctionName("DeleteCalendarItem")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation($"C# HTTP trigger function DeleteCalendarItem processed a request.");
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
            CalendarItem calendarItem = JsonConvert.DeserializeObject<CalendarItem>(requestBody);
            if (String.IsNullOrEmpty(calendarItem.Id))
            {
                return new OkObjectResult(new BackendResult(false, "Die Id des Kalendar-Eintrags fehlt."));
            }
            await _cosmosRepository.DeleteItemAsync(calendarItem.Id);
            // Delete all participants
            IEnumerable<Participant> participants = await _participantRepository.GetItems(p => p.CalendarItemId.Equals(calendarItem.Id));
            foreach (Participant p in participants)
            {
                await _participantRepository.DeleteItemAsync(p.Id);
            }

            // Delete all comments
            IEnumerable<CalendarComment> comments = await _commentRepository.GetItems(c => c.CalendarItemId.Equals(calendarItem.Id));
            foreach (CalendarComment c in comments)
            {
                await _commentRepository.DeleteItemAsync(c.Id);
            }

            return new OkObjectResult(new BackendResult(true));
        }
    }
}

