//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System;
using System.Threading.Tasks;
using Microsoft.MarketplaceServices.ReferenceServiceContracts.V2;
using Microsoft.MarketplaceServicesCore.Core;
using Microsoft.MarketplaceServicesCore.Core.Logging;

namespace Microsoft.MarketplaceServices.ReferenceServiceClient.V2
{
    /// <summary>
    /// Defines operations on the remote service using V1 contracts.
    /// </summary>
    public class ReferenceClientV2 : ReferenceClientBase
    {
        /// <summary>
        /// Gets the supported version.
        /// </summary>
        protected override Version ContractVersion => new Version(2, 0);

        /// <summary>
        /// Constructs the client based on the given settings.
        /// </summary>
        /// <param name="settings">A collection of fields used to configure the service.</param>
        /// <param name="handler">Reference Client Handler.</param>
        public ReferenceClientV2(ReferenceClientSettings settings, ReferenceClientHandler handler)
            : base(settings, handler)
        {
        }

        /// <summary>
        /// Add an entity.
        /// </summary>
        /// <param name="body">The entity to add.</param>
        /// <param name="incomingOperationName">Incoming operation name.</param>
        /// <param name="context">The request context.</param>
        /// <returns>The added entity.</returns>
        public async Task<EntityContractV2> AddEntityV2Async(
            EntityContractV2 body,
            string incomingOperationName,
            LoggingContext context)
        {
            if (body == null)
            {
                throw new ArgumentNullException(nameof(body));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return await this.PostAsync<EntityContractV2>(
                "entities",
                this.GetDependencyRequestLoggingData(
                    incomingOperationName,
                    nameof(AddEntityV2Async),
                    this.ContractVersion.ToString()),
                body,
                context).OnAnyContext();
        }

        /// <summary>
        /// Get entities from the remote service.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="continuationToken">The continuation token.</param>
        /// <param name="incomingOperationName">Incoming operation name.</param>
        /// <param name="context">The request context.</param>
        /// <returns>A page of results.</returns>
        public async Task<PagedEntityContractV2> GetEntitiesV2Async(
            int userId,
            string continuationToken,
            string incomingOperationName,
            LoggingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return await this.GetAsync<PagedEntityContractV2>(
                $"entities?userId={userId}&continuationToken={continuationToken}",
                this.GetDependencyRequestLoggingData(
                    incomingOperationName,
                    nameof(GetEntitiesV2Async),
                    this.ContractVersion.ToString()),
                context).OnAnyContext();
        }

        /// <summary>
        /// Get an entity.
        /// </summary>
        /// <param name="id">The entity id.</param>
        /// <param name="context">The request context.</param>
        /// <param name="incomingOperationName">Incoming operation name.</param>
        /// <returns>The retrieved entity.</returns>
        public async Task<EntityContractV2> GetEntityV2Async(
            string id,
            string incomingOperationName,
            LoggingContext context)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return await this.GetAsync<EntityContractV2>(
                $"entities/{id}",
                this.GetDependencyRequestLoggingData(
                    incomingOperationName,
                    nameof(GetEntitiesV2Async),
                    this.ContractVersion.ToString()),
                context).OnAnyContext();
        }

        /// <summary>
        /// Update an entity.
        /// </summary>
        /// <param name="id">The entity id.</param>
        /// <param name="entity">The entity to add.</param>
        /// <param name="incomingOperationName">Incoming operation name.</param>
        /// <param name="context">The request context.</param>
        /// <returns>The updated entity.</returns>
        public async Task<EntityContractV2> UpdateEntityV2Async(
            string id,
            EntityContractV2 entity,
            string incomingOperationName,
            LoggingContext context)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return await this.PutAsync<EntityContractV2>(
                $"entities/{id}",
                this.GetDependencyRequestLoggingData(
                    incomingOperationName,
                    nameof(UpdateEntityV2Async),
                    this.ContractVersion.ToString()),
                entity,
                context).OnAnyContext();
        }
    }
}