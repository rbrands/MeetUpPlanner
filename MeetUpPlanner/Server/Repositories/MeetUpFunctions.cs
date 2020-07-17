﻿using System;
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

        public MeetUpFunctions(MeetUpFunctionsConfig functionsConfig)
        {
            _functionsConfig = functionsConfig;
        }

        public async Task<string> GetVersion()
        {
            string version = await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/GetVersion"
                            .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                            .GetStringAsync();
            return version;
        }
        public async Task<ClientSettings> GetClientSettings()
        {
            ClientSettings clientSettings = await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/GetClientSettings"
                            .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                            .GetJsonAsync<ClientSettings>();
            return clientSettings;
        }
        public async Task<KeywordCheck> CheckKeyword(string keyword)
        {
            KeywordCheck keywordCheck = await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/CheckKeyword"
                            .SetQueryParam("keyword", keyword)
                            .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                            .GetJsonAsync<KeywordCheck>();
            return keywordCheck;
        }
        public async Task<ServerSettings> GetServerSettings(string adminKeyword)
        {
            ServerSettings serverSettings = await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/GetServerSettings"
                            .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                            .WithHeader(HEADER_KEYWORD, adminKeyword)
                            .GetJsonAsync<ServerSettings>();
            return serverSettings;
        }
        public async Task<IActionResult> WriteServerSettings(string adminKeyword, ServerSettings serverSettings)
        {
            await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/WriteServerSettings"
                            .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                            .WithHeader(HEADER_KEYWORD, adminKeyword)
                            .PostJsonAsync(serverSettings);
            return new OkResult();
        }
        public async Task<IActionResult> WriteClientSettings(string adminKeyword, ClientSettings clientSettings)
        {
            await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/WriteClientSettings"
                            .WithHeader(HEADER_FUNCTIONS_KEY, _functionsConfig.ApiKey)
                            .WithHeader(HEADER_KEYWORD, adminKeyword)
                            .PostJsonAsync(clientSettings);
            return new OkResult();
        }


    }

    public class MeetUpFunctionsConfig
    {
        public string FunctionAppName { get; set; }
        public string ApiKey { get; set; }
    }

}