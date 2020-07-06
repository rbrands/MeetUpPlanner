using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MeetUpPlanner.Shared;

namespace MeetUpFunctions
{
    public static class GetClientSettings
    {
        [FunctionName("GetClientSettings")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function GetClientSettings processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);

            ClientSettings clientSettings = new ClientSettings();
            clientSettings.Title = "Scuderia Südstadt";

            return new OkObjectResult(clientSettings);
        }
    }
}
