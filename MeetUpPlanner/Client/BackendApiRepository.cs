﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Json;
using MeetUpPlanner.Shared;

namespace MeetUpPlanner.Client
{
    public class BackendApiRepository
    {
        private HttpClient _http;
        private const string HEADER_TENANT = "x-meetup-tenant";
        private const string HEADER_KEYWORD = "x-meetup-keyword";
        private AppState _appState;
        public BackendApiRepository(HttpClient http, AppState appState)
        {
            _http = http;
            _appState = appState;
        }
        private void PrepareHttpClient()
        {
            _http.DefaultRequestHeaders.Remove(HEADER_TENANT);
            if (null != _appState.Tenant.TenantKey)
            {
               _http.DefaultRequestHeaders.Add(HEADER_TENANT, _appState.Tenant.TenantKey);
            }
            if (!_http.DefaultRequestHeaders.Contains(HEADER_KEYWORD))
            {
                _http.DefaultRequestHeaders.Add(HEADER_KEYWORD, _appState.KeyWord);
            }
        }


        public async Task<String> GetServerVersion()
        {
            this.PrepareHttpClient();
            String response = await _http.GetStringAsync($"Util/version");
            _http.DefaultRequestHeaders.Remove(HEADER_TENANT);
            return response;
        }
        public async Task<String> GetFunctionVersion()
        {
            this.PrepareHttpClient();
            String response = await _http.GetStringAsync($"Util/functionsVersion");
            _http.DefaultRequestHeaders.Remove(HEADER_TENANT);
            return response;
        }

    }
}