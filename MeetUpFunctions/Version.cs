using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MeetUpPlanner.Functions;


namespace MeetUpFunctions
{
    public static class Version
    {
        const string functionsVersion = Constants.VERSION;
        
        [FunctionName("GetVersion")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            await new StreamReader(req.Body).ReadToEndAsync();

            string responseMessage = functionsVersion;

            return new OkObjectResult(responseMessage);
        }
    }
}
