using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Azure.Cosmos;
using MeetUpPlanner.Shared;

[assembly: FunctionsStartup(typeof(MeetUpPlanner.Functions.Startup))]
namespace MeetUpPlanner.Functions
{
    public class Startup : FunctionsStartup
    {
        /// <summary>
        /// To use a static Cosmos DB client create class for Dependency Injection.
        /// See: https://docs.microsoft.com/en-us/azure/azure-functions/functions-dotnet-dependency-injection
        /// and https://towardsdatascience.com/working-with-azure-cosmos-db-in-your-azure-functions-cc4f0f98a44d
        /// and https://blog.rasmustc.com/azure-functions-dependency-injection/
        /// </summary>
        /// <param name="builder"></param>
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddFilter(level => true);
            });

         

            var config = new ConfigurationBuilder()
                           .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
                           .AddEnvironmentVariables()
                           .Build();

            CosmosClient cosmosClient = new CosmosClient(config["COSMOS_DB_CONNECTION_STRING"]);
            builder.Services.AddSingleton(config);
            builder.Services.AddSingleton(new CosmosDBRepository<ClientSettings>(config, cosmosClient));
            builder.Services.AddSingleton(new CosmosDBRepository<MeetUpPlanner.Shared.TenantSettings>(config, cosmosClient));
            builder.Services.AddSingleton(new CosmosDBRepository<NotificationSubscription>(config, cosmosClient));
            builder.Services.AddSingleton<NotificationSubscriptionRepository>();
            builder.Services.AddSingleton(new CosmosDBRepository<CalendarItem>(config, cosmosClient));
            builder.Services.AddSingleton(new CosmosDBRepository<InfoItem>(config, cosmosClient));
            builder.Services.AddSingleton(new CosmosDBRepository<Participant>(config, cosmosClient));
            builder.Services.AddSingleton(new CosmosDBRepository<CalendarComment>(config, cosmosClient));
            builder.Services.AddSingleton(new CosmosDBRepository<ExportLogItem>(config, cosmosClient));
            builder.Services.AddSingleton(new ServerSettingsRepository(config, cosmosClient));
        }
    }
}
