//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using Newtonsoft.Json;

namespace Microsoft.MarketplaceServices.ReferenceServiceContracts.V2
{
    /// <summary>
    /// Represents the requisite information to page calls.
    /// </summary>
    [JsonObject]
    public sealed class PagingInfoContractV2
    {
        /// <summary>
        /// Gets or sets the continuationToken of this collection.
        /// </summary>
        [JsonProperty(PropertyName = "continuationToken", Required = Required.Default)]
        public string ContinuationToken { get; set; }

        /// <summary>
        /// Gets or sets the total count of the items in this collection.
        /// </summary>
        [JsonProperty(PropertyName = "totalItems", Required = Required.Default)]
        public int TotalItems { get; set; }
    }
}
