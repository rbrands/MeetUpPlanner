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
using System.Collections;

namespace MeetUpPlanner.Functions
{
    public class ExportTrackingReport
    {
        private readonly ILogger _logger;
        private ServerSettingsRepository _serverSettingsRepository;
        private CosmosDBRepository<CalendarItem> _cosmosRepository;
        private CosmosDBRepository<Participant> _participantRepository;
        private CosmosDBRepository<ExportLogItem> _logRepository;

        public ExportTrackingReport(ILogger<ExportTrackingReport> logger, 
                                    ServerSettingsRepository serverSettingsRepository,
                                    CosmosDBRepository<CalendarItem> cosmosRepository,
                                    CosmosDBRepository<Participant> participantRepository,
                                    CosmosDBRepository<ExportLogItem> logRepository)
        {
            _logger = logger;
            _serverSettingsRepository = serverSettingsRepository;
            _cosmosRepository = cosmosRepository;
            _participantRepository = participantRepository;
            _logRepository = logRepository;
        }

        [FunctionName("ExportTrackingReport")]
        [OpenApiOperation(Summary = "Export a list of participants of the given user sharing rides",
                          Description = "All CalendarItems still in database are scanned for participants who had shared an envent with the given person. To be able to read all ExtendedCalenderItems the admin keyword must be set as header x-meetup-keyword.")]
        [OpenApiRequestBody("application/json", typeof(TrackingReportRequest), Description = "Holds all information needed to assemble a tracking report")]
        [OpenApiResponseWithBody(System.Net.HttpStatusCode.OK, "application/json", typeof(TrackingReport))]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function ExportTrackingReport processed a request.");
            string tenant = req.Headers[Constants.HEADER_TENANT];
            if (String.IsNullOrWhiteSpace(tenant))
            {
                tenant = null;
            }
            ServerSettings serverSettings = await _serverSettingsRepository.GetServerSettings(tenant);

            string keyWord = req.Headers[Constants.HEADER_KEYWORD];
            if (String.IsNullOrEmpty(keyWord) || !serverSettings.IsAdmin(keyWord))
            {
                _logger.LogWarning("ExportTrackingReport called with wrong keyword.");
                return new BadRequestErrorMessageResult("Keyword is missing or wrong.");
            }
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            TrackingReportRequest trackingRequest = JsonConvert.DeserializeObject<TrackingReportRequest>(requestBody);
            if (String.IsNullOrEmpty(trackingRequest.RequestorFirstName) || String.IsNullOrEmpty(trackingRequest.RequestorLastName))
            {
                _logger.LogWarning("ExportTrackingReport called without name of requestor.");
                return new BadRequestErrorMessageResult("Requestor name missing.");
            }
            if (String.IsNullOrEmpty(trackingRequest.TrackFirstName) || String.IsNullOrEmpty(trackingRequest.TrackLastName))
            {
                _logger.LogWarning("ExportTrackingReport called without name of person to track.");
                return new BadRequestErrorMessageResult("Track name missing.");
            }
            // Get a list of all CalendarItems
            IEnumerable<CalendarItem> rawListOfCalendarItems;
            if (null == tenant)
            { 
                rawListOfCalendarItems = await _cosmosRepository.GetItems(d => (d.Tenant ?? String.Empty) == String.Empty && !d.IsCanceled);
            }
            else
            {
                rawListOfCalendarItems = await _cosmosRepository.GetItems(d => d.Tenant.Equals(tenant) && !d.IsCanceled);
            }
            List<ExtendedCalendarItem> resultCalendarItems = new List<ExtendedCalendarItem>(50);
            // Filter the CalendarItems that are relevant
            foreach (CalendarItem item in rawListOfCalendarItems)
            {
                // Read all participants for this calendar item
                IEnumerable<Participant> participants = await _participantRepository.GetItems(p => p.CalendarItemId.Equals(item.Id));
                // Only events where the person was part of will be used.
                if (!item.WithoutHost && item.EqualsHost(trackingRequest.TrackFirstName, trackingRequest.TrackLastName) || null != participants.Find(trackingRequest.TrackFirstName, trackingRequest.TrackLastName))
                {
                    ExtendedCalendarItem extendedItem = new ExtendedCalendarItem(item);
                    extendedItem.ParticipantsList = participants;
                    resultCalendarItems.Add(extendedItem);
                }

            }
            IEnumerable<ExtendedCalendarItem> orderedList = resultCalendarItems.OrderBy(d => d.StartDate);
            // Build template for marker list corresponding to orderedList above
            List<CompanionCalendarInfo> relevantCalendarList = new List<CompanionCalendarInfo>(50);
            int calendarSize = 0;
            foreach (ExtendedCalendarItem e in orderedList)
            {
                relevantCalendarList.Add(new CompanionCalendarInfo(e));
                ++calendarSize;
            }

            // Assemble report
            TrackingReport report = new TrackingReport(trackingRequest);
            report.CompanionList = new List<Companion>(50);
            report.CalendarList = relevantCalendarList;
            int calendarIndex = 0;
            foreach (ExtendedCalendarItem calendarItem in orderedList)
            {
                if (!calendarItem.WithoutHost)
                {
                    report.CompanionList.AddCompanion(calendarItem.HostFirstName, calendarItem.HostLastName, calendarItem.HostAdressInfo, calendarSize, calendarIndex);
                }
                foreach (Participant p in calendarItem.ParticipantsList)
                {
                    report.CompanionList.AddCompanion(p.ParticipantFirstName, p.ParticipantLastName, p.ParticipantAdressInfo, calendarSize, calendarIndex);
                }
                ++calendarIndex;
            }
            report.CreationDate = DateTime.Now;
            ExportLogItem log = new ExportLogItem(trackingRequest);
            log.TimeToLive = Constants.LOG_TTL;
            if (null != tenant)
            {
                log.Tenant = tenant;
            }
            await _logRepository.CreateItem(log);

            return new OkObjectResult(report);
        }
    }
}
