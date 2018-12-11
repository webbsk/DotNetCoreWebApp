//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.MarketplaceServices.ReferenceServiceContracts.V1;
using Microsoft.MarketplaceServicesCore.Core;
using Microsoft.MarketplaceServicesCore.Core.Logging;

namespace Microsoft.MarketplaceServices.ReferenceServiceClient.V1
{
    /// <summary>
    /// Defines operations on the remote service using V1 contracts.
    /// </summary>
    public class ReferenceClientV1 : ReferenceClientBase
    {
        /// <summary>
        /// Gets the supported version.
        /// </summary>
        protected override Version ContractVersion => new Version(1, 0);

        /// <summary>
        /// Constructs the client based on the given settings.
        /// </summary>
        /// <param name="settings">A collection of fields used to configure the service.</param>
        /// <param name="handler">Reference Client Handler.</param>
        public ReferenceClientV1(ReferenceClientSettings settings, ReferenceClientHandler handler)
            : base(settings, handler)
        {
        }

        /// <summary>
        /// Add an entity.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <param name="incomingOperationName">Incoming operation name.</param>
        /// <param name="authHeader">Authorization Header</param>
        /// <param name="context">The request context.</param>
        /// <returns>The added entity.</returns>
        public async Task<EntityContractV1> AddEntityV1Async(
            EntityContractV1 entity,
            string incomingOperationName,
            string authHeader,
            LoggingContext context)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return await this.PostAsync<EntityContractV1>(
                "entities",
                this.GetDependencyRequestLoggingData(
                    incomingOperationName,
                    nameof(AddEntityV1Async),
                    this.ContractVersion.ToString()),
                entity,
                context,
                authHeader).OnAnyContext();
        }

        /// <summary>
        /// Get entities from the remote service.
        /// </summary>
        /// <param name="userId">The user id.</param>
        /// <param name="continuationToken">The continuation token.</param>
        /// <param name="incomingOperationName">Incoming operation name.</param>
        /// <param name="context">The request context.</param>
        /// <returns>A page of results.</returns>
        public async Task<PagedEntityContractV1> GetEntitiesV1Async(
            int userId,
            string continuationToken,
            string incomingOperationName,
            LoggingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return await this.GetAsync<PagedEntityContractV1>(
                $"entities?userId={userId}&continuationToken={continuationToken}",
                this.GetDependencyRequestLoggingData(
                    incomingOperationName,
                    nameof(GetEntitiesV1Async),
                    this.ContractVersion.ToString()),
                context).OnAnyContext();
        }

        /// <summary>
        /// Get an entity.
        /// </summary>
        /// <param name="id">The entity id.</param>
        /// <param name="incomingOperationName">Incoming operation name.</param>
        /// <param name="context">The request context.</param>
        /// <returns>The retrieved entity.</returns>
        public async Task<EntityContractV1> GetEntityV1Async(
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

            return await this.GetAsync<EntityContractV1>(
                $"entities/{id}",
                this.GetDependencyRequestLoggingData(
                    incomingOperationName,
                    nameof(GetEntityV1Async),
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
        /// <returns>The added entity.</returns>
        public async Task<EntityContractV1> UpdateEntityV1Async(
            string id,
            EntityContractV1 entity,
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

            return await this.PutAsync<EntityContractV1>(
                $"entities/{id}",
                this.GetDependencyRequestLoggingData(
                    incomingOperationName,
                    nameof(UpdateEntityV1Async),
                    this.ContractVersion.ToString()),
                entity,
                context).OnAnyContext();
        }

        /// <summary>
        /// Get a test response.
        /// </summary>
        /// <param name="httpStatusCode">The status code to return from the service.</param>
        /// <param name="delay">The approximate delay before the response.</param>
        /// <param name="incomingOperationName">Incoming operation name.</param>
        /// <param name="context">The request context.</param>
        /// <returns>The added entity.</returns>
        public async Task<TestContractV1> GetTestV1Async(
            HttpStatusCode httpStatusCode,
            int delay,
            string incomingOperationName,
            LoggingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return await this.GetAsync<TestContractV1>(
                $"test?httpStatusCode={httpStatusCode}&delayInMs={delay}",
                this.GetDependencyRequestLoggingData(
                    incomingOperationName,
                    nameof(GetTestV1Async),
                    this.ContractVersion.ToString()),
                context).OnAnyContext();
        }

        /// <summary>
        /// Get a test response.
        /// </summary>
        /// <param name="httpStatusCode">The status code to return from the service.</param>
        /// <param name="delay">The approximate delay before the response.</param>
        /// <param name="isAutomaticallyRetried">True to retry the request; false otherwise.</param>
        /// <param name="incomingOperationName">Incoming operation name.</param>
        /// <param name="context">The request context.</param>
        /// <returns>The updated entity.</returns>
        public async Task<TestContractV1> GetTestV1Async(
            HttpStatusCode httpStatusCode,
            int delay,
            bool isAutomaticallyRetried,
            string incomingOperationName ,
            LoggingContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return await this.GetAsync<TestContractV1>(
                $"test?httpStatusCode={httpStatusCode}&delayInMs="
                    + $"{delay}&isAutomaticallyRetried={isAutomaticallyRetried}",
                this.GetDependencyRequestLoggingData(
                    incomingOperationName,
                    nameof(GetTestV1Async),
                    this.ContractVersion.ToString()),
                context).OnAnyContext();
        }
    }
}
