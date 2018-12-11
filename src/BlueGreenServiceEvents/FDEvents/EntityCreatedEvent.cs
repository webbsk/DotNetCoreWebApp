//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using Bond;
using Microsoft.MarketplaceServices.Core.Logging;

namespace Microsoft.MarketplaceServices.ReferenceServiceEvents.FDEvents
{
    /// <summary>
    /// Defines an event that occurs when an entity is created.
    /// </summary>
    [Schema]
    [Attribute("SchemaVersion", "3")]
    [Attribute("Description", "Entity created")]
    [Attribute("Provider", "Microsoft.MarketplaceServices.ReferenceServiceEvents.FDEvents")]
    public class EntityCreatedEvent : IncomingServiceRequest
    {
        /// <summary>
        /// The id of the entity that was created.
        /// </summary>
        [Id(10)]
        public string EntityId { get; set; }
    }
}