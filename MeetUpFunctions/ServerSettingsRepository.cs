using System;
using System.Collections.Generic;
using System.Text;
using MeetUpPlanner.Shared;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Cosmos;
using System.Threading.Tasks;


namespace MeetUpPlanner.Functions
{
    public class ServerSettingsRepository : CosmosDBRepository<ServerSettings>
    {
        public ServerSettingsRepository(IConfiguration config, CosmosClient cosmosClient) : base(config, cosmosClient)
        {

        }

        public async Task<ServerSettings> GetServerSettings()
        {
            ServerSettings serverSettings = await this.GetItemByKey(Constants.KEY_SERVER_SETTINGS);
            if (null == serverSettings)
            {
                serverSettings = new ServerSettings()
                {
                    UserKeyword = Constants.DEFAULT_KEYWORD_USER,
                    AdminKeyword = Constants.DEFAULT_KEYWORD_ADMIN,
                    AutoDeleteAfterDays = Constants.DEFAULT_AUTO_DELETE_DAYS
                };
            }
            return serverSettings;
        }

        public async Task<ServerSettings> WriteServerSettings(ServerSettings serverSettings)
        {
            serverSettings.LogicalKey = Constants.KEY_SERVER_SETTINGS;
            return await this.UpsertItem(serverSettings);
        }
    }
}
