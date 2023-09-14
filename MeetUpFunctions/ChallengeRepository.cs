using System;
using System.Collections.Generic;
using System.Text;
using MeetUpPlanner.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace MeetUpPlanner.Functions
{
    public class ChallengeRepository : CosmosDBRepository<StravaSegmentChallenge>
    {
        private readonly ILogger _logger;
        private IConfiguration _config;
        public ChallengeRepository(ILogger<ChallengeRepository> logger, IConfiguration config, CosmosClient cosmosClient) : base(config, cosmosClient)
        {
            _logger = logger;
            _config = config;
            CosmosDbDatabase = _config["COSMOS_DB_DATABASE_BERGFEST"];
            CosmosDbContainer = _config["COSMOS_DB_CONTAINER_BERGFEST"];
        }

        public async Task<StravaSegmentChallenge> GetChallengeByTitle(string challengeTitle)
        {
            try
            {
                if (String.IsNullOrEmpty(challengeTitle))
                {
                    throw new Exception("Missing challengeTitle for call GetChallengeByTitle()");
                }
                string challengeTitleLowerCase = challengeTitle.ToLowerInvariant();
                _logger.LogInformation($"GetChallengeByTitle(ChallengeTitle = {challengeTitleLowerCase}, Container = {CosmosDbContainer})");
                StravaSegmentChallenge challenge = await this.GetFirstItemOrDefault(c => c.UrlTitle == challengeTitleLowerCase);
                return challenge;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                _logger.LogError(ex, "GetChallengeByTitle failed.");
                throw;
            }
        }

    }
}
