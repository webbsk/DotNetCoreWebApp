﻿//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using Bond;
using Microsoft.MarketplaceServices.Core.Logging;

namespace Microsoft.MarketplaceServices.ReferenceServiceEvents.FDEvents
{
    /// <summary>
    /// Defines an event that occurs when an entity is retrieved.
    /// </summary>
    [Schema]
    [Attribute("SchemaVersion", "3")]
    [Attribute("Description", "Entity retrieved")]
    [Attribute("Provider", "Microsoft.MarketplaceServices.ReferenceServiceEvents.FDEvents")]
    public class EntityRetrievedEvent : IncomingServiceRequest
    {
        /// <summary>
        /// The id of the entity that was created.
        /// </summary>
        [Id(10)]
        public string EntityId { get; set; }
    }
}