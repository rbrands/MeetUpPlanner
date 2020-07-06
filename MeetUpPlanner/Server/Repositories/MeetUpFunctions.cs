using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Flurl.Http;
using Flurl;


namespace MeetUpPlanner.Server.Repositories
{
    public class MeetUpFunctions : IMeetUpFunctions
    {
        private MeetUpFunctionsConfig _functionsConfig;

        public MeetUpFunctions(MeetUpFunctionsConfig functionsConfig)
        {
            _functionsConfig = functionsConfig;
        }

        public async Task<string> GetVersion()
        {
            string version = await $"https://{_functionsConfig.FunctionAppName}.azurewebsites.net/api/GetVersion"
                            .WithHeader("x-functions-key", _functionsConfig.ApiKey)
                            .GetStringAsync();
            return version;
        }
    }

    public class MeetUpFunctionsConfig
    {
        public string FunctionAppName { get; set; }
        public string ApiKey { get; set; }
    }

}
