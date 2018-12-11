//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System;
using System.Net.Http;
using Microsoft.MarketplaceServicesCore.Core;

namespace Microsoft.MarketplaceServices.ReferenceServiceEvents.FDEvents
{
    /// <summary>
    /// Extensions class for Reference FD Events.
    /// </summary>
    public static class ReferenceFDEventExtensions
    {
        /// <summary>
        /// Logs entity that was created
        /// </summary>
        /// <param name="request">The http request message</param>
        /// <param name="entityId">Entity id created</param>
        public static void SetEntityCreatedEvent(this HttpRequestMessage request, string entityId)
        {
            if (request == null) { throw new ArgumentNullException(nameof(request)); }

            var eventData = new EntityCreatedEvent
            {
                EntityId = entityId
            };

            request.SetIncomingServiceRequestData(eventData);
        }

        /// <summary>
        /// Logs when entity is retrieved.
        /// </summary>
        /// <param name="request">The http request message</param>
        /// <param name="entityId">Entity id created</param>
        public static void SetEntityRetrievedEvent(this HttpRequestMessage request, string entityId)
        {
            if (request == null) { throw new ArgumentNullException(nameof(request)); }

            var eventData = new EntityRetrievedEvent
            {
                EntityId = entityId
            };

            request.SetIncomingServiceRequestData(eventData);
        }
    }
}
