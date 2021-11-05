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

        public async Task<IEnumerable<ExtendedRoute>> GetRoutes(RouteFilter filter)
        {
            this.PrepareHttpClient();
            HttpResponseMessage response = await _http.PostAsJsonAsync<RouteFilter>($"/api/routes/GetRoutes", filter);
            response.EnsureSuccessStatusCode();
            _http.DefaultRequestHeaders.Remove(HEADER_TENANT);
            return await response.Content.ReadFromJsonAsync<IEnumerable<ExtendedRoute>>();
        }


    }
}
