using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace MeetUpPlanner.Shared
{
    public class CosmosDBEntity
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        [JsonProperty(PropertyName = "type")]
        public string Type
        {
            get
            {
                return this.GetType().Name;
            }
            set
            {

            }
        }
        [JsonProperty(PropertyName = "partitionKey")]
        public string PartitionKey
        {
            get
            {
                return this.Type;
            }
            set
            {

            }
        }
        /// <summary>
        /// Logical key to be used for this entity. If this member is set the id of the document
        /// is created as according to the pattern type-key and should be unique.
        /// </summary>
        [JsonProperty(PropertyName = "key", NullValueHandling = NullValueHandling.Ignore)]
        public string Key { get; set; }
        public string LogicalKey 
        { 
            get
            { 
                return Key;  
            }
            set
            {
                Key = value;
            }
        }
        // used to set Time-to-live for expiration policy
        [JsonProperty(PropertyName = "ttl", NullValueHandling = NullValueHandling.Ignore)]
        public int? TimeToLive { get; set; }
        [JsonProperty(PropertyName = "tenant", NullValueHandling = NullValueHandling.Ignore)]
        public string Tenant { get; set; }
        public void SetUniqueKey()
        {
            if (null != LogicalKey)
            {
                Id = Type + "-" + LogicalKey;
            }
            else
            {
                Id = Guid.NewGuid().ToString();
            }
        }
    }
}
