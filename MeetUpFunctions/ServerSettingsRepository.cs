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
        private string _inviteGuestKey;
        public ServerSettingsRepository(IConfiguration config, CosmosClient cosmosClient) : base(config, cosmosClient)
        {
            _inviteGuestKey = config[Constants.INVITE_GUEST_KEY_CONFIG];
        }
        public string InviteGuestKey { get;  }

        public bool IsInvitedGuest(string keyword)
        {
            return _inviteGuestKey.Equals(keyword);
        }
        public async Task<ServerSettings> GetServerSettings()
        {
            return await GetServerSettings(null);
        }
        public async Task<ServerSettings> GetServerSettings(string tenant)
        {
            string settingsKey = Constants.KEY_SERVER_SETTINGS;
            if (!String.IsNullOrWhiteSpace(tenant))
            {
                settingsKey += "-" + tenant;
            }
            ServerSettings serverSettings = await this.GetItemByKey(settingsKey);
            if (null == serverSettings)
            {
                serverSettings = new ServerSettings()
                {
                    UserKeyword = Constants.DEFAULT_KEYWORD_USER,
                    AdminKeyword = Constants.DEFAULT_KEYWORD_ADMIN,
                    AutoDeleteAfterDays = Constants.DEFAULT_AUTO_DELETE_DAYS,
                    Tenant = tenant
                };
            }
            return serverSettings;
        }

        public async Task<ServerSettings> WriteServerSettings(ServerSettings serverSettings)
        {
            serverSettings.LogicalKey = Constants.KEY_SERVER_SETTINGS;
            if (!String.IsNullOrWhiteSpace(serverSettings.Tenant))
            {
                serverSettings.LogicalKey += "-" + serverSettings.Tenant;
            }
            return await this.UpsertItem(serverSettings);
        }
    }
}
