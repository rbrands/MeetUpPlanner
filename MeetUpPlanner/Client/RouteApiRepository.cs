using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using MeetUpPlanner.Shared;

namespace MeetUpPlanner.Client
{
    public class RouteApiRepository
    {
        private HttpClient _http;
        private const string HEADER_TENANT = "x-meetup-tenant";
        private const string HEADER_KEYWORD = "x-meetup-keyword";
         private AppState _appState;
        public RouteApiRepository(HttpClient http, AppState appState)
        {
            _http = http;
            _appState = appState;
        }
        private void PrepareHttpClient()
        {
            _http.DefaultRequestHeaders.Remove(HEADER_TENANT);
            _http.DefaultRequestHeaders.Add(HEADER_TENANT, _appState.Tenant.TrackKey);
            if (!_http.DefaultRequestHeaders.Contains(HEADER_KEYWORD))
            {
                _http.DefaultRequestHeaders.Add(HEADER_KEYWORD, _appState.KeyWord);
            }
        }


        public async Task<String> GetVersion()
        {
            this.PrepareHttpClient();
            String response = await _http.GetStringAsync($"/api/routes/GetVersion");
            _http.DefaultRequestHeaders.Remove(HEADER_TENANT);
            return response;
        }
        public async Task<IEnumerable<ExtendedRoute>> GetRoutes(RouteFilter filter)
        {
            this.PrepareHttpClient();
            HttpResponseMessage response = await _http.PostAsJsonAsync<RouteFilter>($"/api/routes/GetRoutes", filter);
            response.EnsureSuccessStatusCode();
            _http.DefaultRequestHeaders.Remove(HEADER_TENANT);
            return await response.Content.ReadFromJsonAsync<IEnumerable<ExtendedRoute>>();
        }
        public async Task<IEnumerable<TagSet>> GetTagSets()
        {
            this.PrepareHttpClient();
            IEnumerable<TagSet> response = await _http.GetFromJsonAsync<IEnumerable<TagSet>>($"/api/routes/GetTagSets");
            _http.DefaultRequestHeaders.Remove(HEADER_TENANT);
            return response;
        }
        public async Task<Comment> WriteComment(Comment comment)
        {
            this.PrepareHttpClient();
            HttpResponseMessage response = await _http.PostAsJsonAsync<Comment>($"/api/routes/WriteComment", comment);
            response.EnsureSuccessStatusCode();
            _http.DefaultRequestHeaders.Remove(HEADER_TENANT);
            return await response.Content.ReadFromJsonAsync<Comment>();
        }


    }
}
