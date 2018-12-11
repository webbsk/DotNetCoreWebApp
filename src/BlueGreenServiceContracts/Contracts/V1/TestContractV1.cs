//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System.Net;
using Newtonsoft.Json;

namespace Microsoft.MarketplaceServices.ReferenceServiceContracts.V1
{
    /// <summary>
    /// The V1 contract for a "test" contract.
    /// </summary>
    [JsonObject]
    public sealed class TestContractV1
    {
        /// <summary>
        /// Gets or sets the http status code.
        /// </summary>
        [JsonProperty(PropertyName = "httpStatusCode", Required = Required.Default)]
        public HttpStatusCode HttpStatusCode { get; set; }

        /// <summary>
        /// Gets or sets the delay in milli seconds.
        /// </summary>
        [JsonProperty(PropertyName = "delayInMilliseconds", Required = Required.Default)]
        public int DelayInMilliseconds { get; set; }

        /// <summary>
        /// Gets or sets whether the downstream request is automatically retried.
        /// </summary>
        [JsonProperty(PropertyName = "isAutomaticallyRetried")]
        public bool IsAutomaticallyRetried { get; set; }
    }
}
