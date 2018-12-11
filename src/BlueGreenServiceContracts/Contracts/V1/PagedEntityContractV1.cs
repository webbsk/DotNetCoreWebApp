//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System.Collections.Generic;
using Newtonsoft.Json;

namespace Microsoft.MarketplaceServices.ReferenceServiceContracts.V1
{
    /// <summary>
    /// The V1 contract for a paged set of entities.
    /// </summary>
    [JsonObject]
    public sealed class PagedEntityContractV1
    {
        readonly IList<EntityContractV1> items = new List<EntityContractV1>();

        /// <summary>
        /// Gets or sets information related to paging.
        /// </summary>
        [JsonProperty(PropertyName = "pagingInfo", Required = Required.Always)]
        public PagingInfoContractV1 PagingInfo { get; set; }

        /// <summary>
        /// Gets the items in this page of the entities.
        /// </summary>
        [JsonProperty(PropertyName = "items", Required = Required.Default)]
        public IList<EntityContractV1> Items { get => this.items; }
    }
}
