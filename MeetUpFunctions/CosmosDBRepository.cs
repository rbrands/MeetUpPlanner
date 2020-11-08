using MeetUpPlanner.Shared;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Azure.Cosmos.Linq;



namespace MeetUpPlanner.Functions
{
    public class CosmosDBRepository<T> where T : CosmosDBEntity, new()
    {
        private IConfiguration _config;
        private CosmosClient _cosmosClient;
        string _cosmosDbDatabase;
        string _cosmosDbContainer;

        /// <summary>
        /// Create repository, typically as singleton. Create CosmosClient before.
        /// </summary>
        /// <param name="cosmosClient"></param>
        /// <param name="config"></param>
        public CosmosDBRepository(IConfiguration config, CosmosClient cosmosClient)
        {
            _config = config;
            _cosmosClient = cosmosClient;
            _cosmosDbDatabase = _config["COSMOS_DB_DATABASE"];
            _cosmosDbContainer = _config["COSMOS_DB_CONTAINER"];
        }
        public async Task<T> CreateItem(T item)
        {
            if (null == item)
            {
                throw new ArgumentNullException("item");
            }
            item.SetUniqueKey();
            return await _cosmosClient.GetDatabase(_cosmosDbDatabase)
                                      .GetContainer(_cosmosDbContainer)
                                      .CreateItemAsync<T>(item, new PartitionKey(item.PartitionKey));
        }
        public async Task DeleteItemAsync(string id)
        {
            await _cosmosClient.GetDatabase(_cosmosDbDatabase)
                               .GetContainer(_cosmosDbContainer)
                               .DeleteItemAsync<T>(id, new PartitionKey(typeof(T).Name));
        }

        public async Task<T> GetItem(string id)
        {
            PartitionKey partitionKey = new PartitionKey(typeof(T).Name);
            if (String.IsNullOrEmpty(id))
            {
                throw new ApplicationException("Missing document id.");
            }
            try
            {
                ItemResponse<T> item = await _cosmosClient.GetDatabase(_cosmosDbDatabase)
                                                          .GetContainer(_cosmosDbContainer)
                                                          .ReadItemAsync<T>(id, partitionKey);

                return item.Resource;
            }
            catch (CosmosException e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }
        /// <summary>
        /// Create a key for the document that uses the type as prefix and the given argument as suffix.
        /// The composed key should be unique because it it used as id.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> GetItemByKey(string key)
        {
            if (String.IsNullOrEmpty(key))
            {
                throw new ApplicationException("Missing item key.");
            }
            string id = typeof(T).Name + "-" + key;
            return await GetItem(id);
        }
        public async Task<IEnumerable<T>> GetItems(Expression<Func<T, bool>> predicate, int maxItemCount = -1)
        {
            Container container = _cosmosClient.GetDatabase(_cosmosDbDatabase).GetContainer(_cosmosDbContainer);
            PartitionKey partitionKey = new PartitionKey(typeof(T).Name);

            FeedIterator<T> itemIterator = container.GetItemLinqQueryable<T>(true, null, new QueryRequestOptions { MaxItemCount = maxItemCount, PartitionKey = partitionKey })
                                             .Where(d => d.Type == typeof(T).Name)
                                             .Where<T>(predicate)
                                             .ToFeedIterator<T>();
            List<T> results = new List<T>();
            while (itemIterator.HasMoreResults)
            {
                results.AddRange(await itemIterator.ReadNextAsync());
            }
            return results;
        }
        /// <summary>
        ///  Gets the first item matching the given condition, null if not found.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public async Task<T> GetFirstItemOrDefault(Expression<Func<T, bool>> predicate)
        {
            Container container = _cosmosClient.GetDatabase(_cosmosDbDatabase).GetContainer(_cosmosDbContainer);
            PartitionKey partitionKey = new PartitionKey(typeof(T).Name);

            FeedIterator<T> itemIterator = container.GetItemLinqQueryable<T>(true, null, new QueryRequestOptions { MaxItemCount = 1, PartitionKey = partitionKey })
                                             .Where(d => d.Type == typeof(T).Name)
                                             .Where<T>(predicate)
                                             .ToFeedIterator<T>();
            List<T> results = new List<T>();
            while (itemIterator.HasMoreResults)
            {
                results.AddRange(await itemIterator.ReadNextAsync());
            }
            T firstItem = results.FirstOrDefault();
            return firstItem;
        }

        public async Task<IEnumerable<T>> GetItems()
        {
            Container container = _cosmosClient.GetDatabase(_cosmosDbDatabase).GetContainer(_cosmosDbContainer);
            PartitionKey partitionKey = new PartitionKey(typeof(T).Name);

            FeedIterator<T> itemIterator = container.GetItemLinqQueryable<T>(true, null, new QueryRequestOptions { PartitionKey = partitionKey })
                                             .Where(d => d.Type == typeof(T).Name)
                                             .ToFeedIterator<T>();
            List<T> results = new List<T>();
            while (itemIterator.HasMoreResults)
            {
                results.AddRange(await itemIterator.ReadNextAsync());
            }
            return results;
        }

        public async Task<T> UpsertItem(T item)
        {
            Container container = _cosmosClient.GetDatabase(_cosmosDbDatabase).GetContainer(_cosmosDbContainer);
            PartitionKey partitionKey = new PartitionKey(typeof(T).Name);

            if (null == item)
            {
                throw new ArgumentNullException("item");
            }
            if (null == item.Id)
            {
                item.SetUniqueKey();
            }
            ItemResponse<T> response = await container.UpsertItemAsync<T>(item, partitionKey);

            return response.Resource;
        }


    }
}
