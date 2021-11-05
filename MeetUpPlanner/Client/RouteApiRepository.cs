using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;

namespace MeetUpPlanner.Client
{
    public class RouteApiRepository
    {
        private HttpClient _http;
        private AppState _appState;
        public RouteApiRepository(HttpClient http, AppState appState)
        {
            _http = http;
            _appState = appState;
        }
        private void PrepareHttpClient()
        {
            _http.DefaultRequestHeaders.Remove("x-meetup-tenant");
            if (!String.IsNullOrEmpty(_appState.Tenant.TrackKey))
            {
                _http.DefaultRequestHeaders.Add("x-meetup-tenant", _appState.Tenant.TrackKey);
            }
        }

    }
}
