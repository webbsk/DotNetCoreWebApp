//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System;

namespace Microsoft.MarketplaceServices.ReferenceServiceConfiguration.Service
{
    /// <summary>
    /// Service Throttle configuration
    /// </summary>
    public class ThrottleConfiguration
    {
        /// <summary>
        /// Gets or sets the maximum accepts.
        /// </summary>
        public int MaxAccepts { get; set; }

        /// <summary>
        /// Gets or sets the maximum connections.
        /// </summary>
        public long MaxConnections { get; set; }

        /// <summary>
        /// Gets or sets the request queue limit
        /// </summary>
        public long RequestQueueLimit { get; set; }

        /// <summary>
        /// Time for which queue can remain in queue before being picked up by application
        /// </summary>
        public TimeSpan RequestQueueTimeout { get; set; }
    }
}
