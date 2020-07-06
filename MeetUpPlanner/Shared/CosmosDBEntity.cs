using System;
using System.Collections.Generic;
using System.Text;

namespace MeetUpPlanner.Shared
{
    public class CosmosDBEntity
    {
        public string Id { get; set; }
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
        public string LogicalKey { get; set; }
        // used to set Time-to-live for expiration policy
        public int? Ttl { get; set; }
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
