using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl;
using MeetUpPlanner.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Net.Http;

namespace MeetUpPlanner.Server.Repositories
{
    public class MeetUpFunctions
    {
        private readonly MeetUpFunctionsConfig _functionsConfig;
        public const string HEADER_KEYWORD = "x-meetup-keyword";
        public const string HEADER_FUNCTIONS_KEY = "x-functions-key";
        public const string HEADER_TENANT = "x-meetup-tenant";
        public const string HEADER_TENANT_URL = "x-meetup-tenant-url";

        public MeetUpFunctions(MeetUpFunctionsConfig functionsConfig)
        {
            _functionsConfig = functionsConfig;
        }
        public string InviteGuestKey
        {
            get { return _functionsConfig.InviteGuestKey; }
        }

        public async Task<string> GetVersion()
        {
            string version = await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/GetVersion"
                            .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                            .GetStringAsync();
            return version;
        }
        public async Task<TenantClientSettings> GetTenantClientSettings(string tenantUrl)
        {
            TenantClientSettings tenantClientSettings;
            tenantClientSettings = await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/GetTenantClientSettings"
                            .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                            .WithHeader(HEADER_TENANT_URL, tenantUrl)
                            .GetJsonAsync<TenantClientSettings>();
            return tenantClientSettings;
        }
        public async Task<ClientSettings> GetClientSettings(string tenant)
        {
            ClientSettings clientSettings; 
            clientSettings = await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/GetClientSettings"
                            .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                            .WithHeader(HEADER_TENANT, tenant)
                            .GetJsonAsync<ClientSettings>();
            return clientSettings;
        }
        public async Task<KeywordCheck> CheckKeyword(string tenant, string keyword)
        {
            KeywordCheck keywordCheck;
            keywordCheck = await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/CheckKeyword"
                            .WithHeader(HEADER_KEYWORD, keyword)
                            .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                            .WithHeader(HEADER_TENANT, tenant)
                            .GetJsonAsync<KeywordCheck>();
            return keywordCheck;
        }
        public async Task<ServerSettings> GetServerSettings(string tenant, string adminKeyword)
        {
            ServerSettings serverSettings;
            serverSettings = await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/GetServerSettings"
                            .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                            .WithHeader(HEADER_KEYWORD, adminKeyword)
                            .WithHeader(HEADER_TENANT, tenant)
                            .GetJsonAsync<ServerSettings>();
            return serverSettings;
        }
        public async Task<IActionResult> WriteServerSettings(string tenant, string adminKeyword, ServerSettings serverSettings)
        {
            await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/WriteServerSettings"
                            .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                            .WithHeader(HEADER_KEYWORD, adminKeyword)
                            .WithHeader(HEADER_TENANT, tenant)
                            .PostJsonAsync(serverSettings);

            return new OkResult();
        }
        public async Task<IActionResult> WriteClientSettings([FromHeader(Name = "x-meetup-tenant")] string tenant, [FromHeader(Name = "x-meetup-keyword")] string adminKeyword, ClientSettings clientSettings)
        {
            await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/WriteClientSettings"
                            .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                            .WithHeader(HEADER_KEYWORD, adminKeyword)
                            .WithHeader(HEADER_TENANT, tenant)
                            .PostJsonAsync(clientSettings);
            return new OkResult();
        }

        public async Task<IActionResult> WriteCalendarItem(string tenant, string keyword, CalendarItem calendarItem)
        {
            await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/WriteCalendarItem"
                            .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                            .WithHeader(HEADER_KEYWORD, keyword)
                            .WithHeader(HEADER_TENANT, tenant)
                            .PostJsonAsync(calendarItem);

            return new OkResult();
        }
        public async Task<IActionResult> WriteNotificationSubscription(string tenant, string keyword, NotificationSubscription subscription)
        {
            await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/WriteNotificationSubscription"
                            .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                            .WithHeader(HEADER_KEYWORD, keyword)
                            .WithHeader(HEADER_TENANT, tenant)
                            .PostJsonAsync(subscription );

            return new OkResult();
        }
        public async Task<IActionResult> WriteInfoItem(string tenant, string keyword, InfoItem infoItem)
        {
            await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/WriteInfoItem"
                            .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                            .WithHeader(HEADER_KEYWORD, keyword)
                            .WithHeader(HEADER_TENANT, tenant)
                            .PostJsonAsync(infoItem);

            return new OkResult();
        }
        public async Task<IEnumerable<CalendarItem>> GetCalendarItems(string tenant, string keyword, string privatekeywords)
        {

            IEnumerable<CalendarItem> calendarItems = await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/GetCalendarItems"
                          .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                          .WithHeader(HEADER_KEYWORD, keyword)
                          .WithHeader(HEADER_TENANT, tenant)
                          .SetQueryParam("privatekeywords", privatekeywords)
                          .GetJsonAsync<IEnumerable<CalendarItem>>();
            return calendarItems;
        }
        public async Task<IEnumerable<ExtendedCalendarItem>> GetExtendedCalendarItems(string tenant, string keyword, string privatekeywords)
        {
            IEnumerable<ExtendedCalendarItem> calendarItems;
            calendarItems = await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/GetExtendedCalendarItems"
                            .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                            .WithHeader(HEADER_KEYWORD, keyword)
                            .WithHeader(HEADER_TENANT, tenant)
                            .SetQueryParam("privatekeywords", privatekeywords)
                            .GetJsonAsync<IEnumerable<ExtendedCalendarItem>>();
            return calendarItems;
        }
        public async Task<IEnumerable<ExtendedInfoItem>> GetExtendedInfoItems(string tenant, string keyword)
        {
            IEnumerable<ExtendedInfoItem> infoItems;
            infoItems = await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/GetExtendedInfoItems"
                            .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                            .WithHeader(HEADER_KEYWORD, keyword)
                            .WithHeader(HEADER_TENANT, tenant)
                            .GetJsonAsync<IEnumerable<ExtendedInfoItem>>();
            return infoItems;
        }
        public async Task<IEnumerable<ExtendedCalendarItem>> GetExtendedCalendarItemsForDate(string tenant, string keyword, string privatekeywords, string requestedDate)
        {
            IEnumerable<ExtendedCalendarItem> calendarItems;
            calendarItems = await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/GetExtendedCalendarItemsForDate"
                            .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                            .WithHeader(HEADER_KEYWORD, keyword)
                            .WithHeader(HEADER_TENANT, tenant)
                            .SetQueryParam("privatekeywords", privatekeywords)
                            .SetQueryParam("requesteddate", requestedDate)
                            .GetJsonAsync<IEnumerable<ExtendedCalendarItem>>();
            return calendarItems;
        }
        public async Task<IEnumerable<ExtendedCalendarItem>> GetScopedCalendarItems(string tenant, string scope)
        {
            IEnumerable<ExtendedCalendarItem> calendarItems;
            calendarItems = await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/GetScopedCalendarItems"
                            .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                            .WithHeader(HEADER_TENANT, tenant)
                            .SetQueryParam("scope", scope)
                            .GetJsonAsync<IEnumerable<ExtendedCalendarItem>>();
            return calendarItems;
        }
        public async Task<CalendarItem> GetCalendarItem(string tenant, string keyword, string itemId)
        {

            CalendarItem calendarItem = await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/GetCalendarItem/{itemId}"
                          .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                          .WithHeader(HEADER_KEYWORD, keyword)
                          .WithHeader(HEADER_TENANT, tenant)
                          .GetJsonAsync<CalendarItem>();
            return calendarItem;
        }
        public async Task<InfoItem> GetInfoItem(string tenant, string keyword, string itemId)
        {

            InfoItem infoItem = await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/GetInfoItem/{itemId}"
                          .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                          .WithHeader(HEADER_KEYWORD, keyword)
                          .WithHeader(HEADER_TENANT, tenant)
                          .GetJsonAsync<InfoItem>();
            return infoItem;
        }

        public async Task<ExtendedCalendarItem> GetExtendedCalendarItem(string tenant, string keyword, string itemId)
        {

            ExtendedCalendarItem calendarItem = await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/GetExtendedCalendarItem/{itemId}"
                          .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                          .WithHeader(HEADER_KEYWORD, keyword)
                          .WithHeader(HEADER_TENANT, tenant)
                          .GetJsonAsync<ExtendedCalendarItem>();
            return calendarItem;
        }
        public async Task<ExtendedInfoItem> GetExtendedInfoItem(string tenant, string keyword, string itemId)
        {

            ExtendedInfoItem infoItem = await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/GetExtendedInfoItem/{itemId}"
                          .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                          .WithHeader(HEADER_KEYWORD, keyword)
                          .WithHeader(HEADER_TENANT, tenant)
                          .GetJsonAsync<ExtendedInfoItem>();
            return infoItem;
        }
        public async Task<BackendResult> AddParticipantToCalendarItem(string tenant, string keyword, Participant participant)
        {
            BackendResult result = await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/AddParticipantToCalendarItem"
                            .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                            .WithHeader(HEADER_KEYWORD, keyword)
                            .WithHeader(HEADER_TENANT, tenant)
                            .PostJsonAsync(participant)
                            .ReceiveJson<BackendResult>();
            return result;
        }
        public async Task<BackendResult> AddCommentToCalendarItem(string tenant, string keyword, CalendarComment comment)
        {
            BackendResult result;
            result = await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/AddCommentToCalendarItem"
                        .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                        .WithHeader(HEADER_KEYWORD, keyword)
                        .WithHeader(HEADER_TENANT, tenant)
                        .PostJsonAsync(comment)
                        .ReceiveJson<BackendResult>();

            return result;
        }
        public async Task<BackendResult> AddCommentToInfoItem(string tenant, string keyword, CalendarComment comment)
        {
            BackendResult result;
            result = await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/AddCommentToInfoItem"
                        .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                        .WithHeader(HEADER_KEYWORD, keyword)
                        .WithHeader(HEADER_TENANT, tenant)
                        .PostJsonAsync(comment)
                        .ReceiveJson<BackendResult>();

            return result;
        }
        public async Task<BackendResult> RemoveParticipantFromCalendarItem(string tenant, string keyword, Participant participant)
        {
            BackendResult result = await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/RemoveParticipantFromCalendarItem"
                            .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                            .WithHeader(HEADER_KEYWORD, keyword)
                            .WithHeader(HEADER_TENANT, tenant)
                            .PostJsonAsync(participant)
                            .ReceiveJson<BackendResult>();
            return result;
        }
        public async Task<BackendResult> AssignNewHostToCalendarItem(string tenant, string keyword, Participant participant)
        {
            BackendResult result = await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/AssignNewHostToCalendarItem"
                            .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                            .WithHeader(HEADER_KEYWORD, keyword)
                            .WithHeader(HEADER_TENANT, tenant)
                            .PostJsonAsync(participant)
                            .ReceiveJson<BackendResult>();
            return result;
        }
        public async Task<BackendResult> RemoveCommentFromCalendarItem(string tenant, string keyword, CalendarComment comment)
        {
            BackendResult result = await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/RemoveCommentFromCalendarItem"
                            .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                            .WithHeader(HEADER_KEYWORD, keyword)
                            .WithHeader(HEADER_TENANT, tenant)
                            .PostJsonAsync(comment)
                            .ReceiveJson<BackendResult>();
            return result;
        }
        public async Task<BackendResult> DeleteCalendarItem(string tenant, string keyword, CalendarItem calendarItem)
        {
            BackendResult result = await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/DeleteCalendarItem"
                            .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                            .WithHeader(HEADER_KEYWORD, keyword)
                            .WithHeader(HEADER_TENANT, tenant)
                            .PostJsonAsync(calendarItem)
                            .ReceiveJson<BackendResult>();
            return result;
        }
        public async Task<BackendResult> DeleteInfoItem(string tenant, string keyword, InfoItem infoItem)
        {
            BackendResult result = await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/DeleteInfoItem"
                            .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                            .WithHeader(HEADER_KEYWORD, keyword)
                            .WithHeader(HEADER_TENANT, tenant)
                            .PostJsonAsync(infoItem)
                            .ReceiveJson<BackendResult>();
            return result;
        }
        public async Task<TrackingReport> ExportTrackingReport(string tenant, string keyword, TrackingReportRequest request)
        {
            TrackingReport result = await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/ExportTrackingReport"
                            .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                            .WithHeader(HEADER_KEYWORD, keyword)
                            .WithHeader(HEADER_TENANT, tenant)
                            .PostJsonAsync(request)
                            .ReceiveJson<TrackingReport>();
            return result;
        }
        public async Task<IEnumerable<ExportLogItem>> GetExportLog(string tenant, string keyword)
        {
            IEnumerable<ExportLogItem> result = await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/GetExportLog"
                            .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                            .WithHeader(HEADER_KEYWORD, keyword)
                            .WithHeader(HEADER_TENANT, tenant)
                            .GetJsonAsync<IEnumerable<ExportLogItem>>();
            return result;
        }

    }

    public class MeetUpFunctionsConfig
    {
        public string FunctionAppName { get; set; }
        public string ApiKey { get; set; }
        public string InviteGuestKey { get; set; }
    }

}
