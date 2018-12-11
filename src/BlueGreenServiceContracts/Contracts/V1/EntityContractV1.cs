//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using Newtonsoft.Json;

namespace Microsoft.MarketplaceServices.ReferenceServiceContracts.V1
{
    /// <summary>
    /// The V1 contract for an "entity".
    /// </summary>
    [JsonObject]
    public sealed class EntityContractV1
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Default)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the entity.
        /// </summary>
        [JsonProperty(PropertyName = "name", Required = Required.Always)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the count associated with this entity.
        /// </summary>
        [JsonProperty(PropertyName = "count", Required = Required.Default)]
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets an arbitrary BLOB for the client to associate data with the entity.
        /// </summary>
        [JsonProperty(PropertyName = "blob", Required = Required.Default)]
        public string Blob { get; set; }

        /// <summary>
        /// The id of the user this entity belongs to.
        /// </summary>
        [JsonProperty(PropertyName = "userId", Required = Required.Default)]
        public int UserId { get; set; }
    }
}
