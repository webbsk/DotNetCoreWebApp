//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System;

namespace Microsoft.MarketplaceServices.ReferenceServiceClient
{
    /// <summary>
    /// Configurable Reference Client Settings
    /// </summary>
    public class ReferenceClientSettings
    {
        /// <summary>
        /// Base Uri of the Reference Service
        /// </summary>
        public Uri BaseAddress { get; set; }

        /// <summary>
        /// Name of this service for logging purposes
        /// </summary>
        public string CallingServiceName { get; set; }

        /// <summary>
        /// Gets or sets whether ssl errors should be ignored
        /// Only to be used in units tests where self signed certificate is used.
        /// </summary>
        public bool IgnoreSSLErrors { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the request and
        /// response payloads should be traced for successful requests.
        /// </summary>
        public bool LogPayloadForSuccessfulRequests { get; set; }

        /// <summary>
        /// Connection timeout
        /// </summary>
        public TimeSpan Timeout { get; set; }
    }
}
