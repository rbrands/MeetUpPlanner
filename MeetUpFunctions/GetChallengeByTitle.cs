using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using MeetUpPlanner.Shared;
using System.Web.Http;
using Aliencube.AzureFunctions.Extensions.OpenApi.Core.Attributes;


namespace MeetUpPlanner.Functions
{
    public class GetChallengeByTitle
    {
        private readonly ILogger _logger;
        private ChallengeRepository _cosmosRepository;

        public GetChallengeByTitle(ILogger<GetCalendarItem> logger, ServerSettingsRepository serverSettingsRepository, ChallengeRepository cosmosRepository)
        {
            _logger = logger;
            _cosmosRepository = cosmosRepository;
        }

        [FunctionName("GetChallengeByTitle")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "GetChallengeByTitle/{challengeTitle}")] HttpRequest req,
            string challengeTitle)
        {
            try
            {
                if (String.IsNullOrEmpty(challengeTitle))
                {
                    throw new Exception("Missing challengeTitle for call GetChallenge()");
                }
                StravaSegmentChallenge challenge = await _cosmosRepository.GetChallengeByTitle(challengeTitle);
                return new OkObjectResult(challenge);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"GetChallenge(challengeTitle = {challengeTitle}) failed.");
                return new BadRequestErrorMessageResult(ex.Message);
            }
        }
    }
}
