//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using Newtonsoft.Json;

namespace Microsoft.MarketplaceServices.ReferenceServiceContracts.V2
{
    /// <summary>
    /// The V2 contract for an "entity".
    /// This contract has the following differences from the V1 contract:
    ///   -The "count" field is renamed to "number"
    ///   -The previously required "name" field is removed
    /// </summary>
    [JsonObject]
    public sealed class EntityContractV2
    {
        /// <summary>
        /// Gets or sets the unique identifier for the entity.
        /// </summary>
        [JsonProperty(PropertyName = "id", Required = Required.Default)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the number associated with this entity.
        /// </summary>
        [JsonProperty(PropertyName = "number", Required = Required.Default)]
        public int Number { get; set; }

        /// <summary>
        /// Gets or sets an arbitrary BLOB for the client to associate data with the entity.
        /// </summary>
        [JsonProperty(PropertyName = "blob", Required = Required.Default)]
        public string Blob { get; set; }

        /// <summary>
        /// The user the entity belongs to.
        /// </summary>
        [JsonProperty(PropertyName = "userId")]
        public int UserId { get; set; }
    }
}
