//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.MarketplaceServices.ReferenceServiceContracts.V2
{
    /// <summary>
    /// The V2 contract for a paged set of entities.
    /// </summary>
    [JsonObject]
    public sealed class PagedEntityContractV2
    {
        readonly IList<EntityContractV2> items = new List<EntityContractV2>();

        /// <summary>
        /// Gets or sets information related to paging.
        /// </summary>
        [JsonProperty(PropertyName = "pagingInfo", Required = Required.Always)]
        public PagingInfoContractV2 PagingInfo { get; set; }

        /// <summary>
        /// Gets the items in this page of the entities.
        /// </summary>
        [JsonProperty(PropertyName = "items", Required = Required.Default)]
        public IList<EntityContractV2> Items { get =>  this.items; }
    }
}
