//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.MarketplaceServices.ReferenceServiceAuthorization;
using Microsoft.MarketplaceServices.ReferenceServiceContracts.V1;
using Microsoft.MarketplaceServices.ReferenceServiceEvents.FDEvents;
using Microsoft.MarketplaceServices.ReferenceServiceExceptions;
using Microsoft.MarketplaceServicesCore.Authentication;
using Microsoft.MarketplaceServicesCore.Authentication.Aad;
using Microsoft.MarketplaceServicesCore.Core.Counters;

namespace Microsoft.MarketplaceServices.ReferenceServiceControllers.V1
{
    /// <summary>
    /// Defines V1 operations for the entity resources.
    /// </summary>
    [Route("v1.0")]
    [AadAuthenticationFilter(profile: "Default")]
    [ClaimExistsRequirement(claimType: new[] { AuthenticationClaimTypes.Id, AuthenticationClaimTypes.Partner })]
    public sealed class EntityControllerV1 : ApiController
    {
        readonly ICounterFactory counterfactory;
        readonly ReferenceServiceExceptionProvider exceptionProvider;

        /// <summary>
        /// Creates the default entity controller.
        /// </summary>
        /// <param name="logger">Sequenced Event logger.</param>
        /// <param name="counterfactory">Counterfactory to use.</param>
        /// <param name="exceptionProvider">Error Provider.</param>
        public EntityControllerV1(
            ICounterFactory counterfactory,
            ReferenceServiceExceptionProvider exceptionProvider)
        {
            this.counterfactory = counterfactory ?? throw new ArgumentNullException(nameof(counterfactory));
            this.exceptionProvider = exceptionProvider ?? throw new ArgumentNullException(nameof(exceptionProvider));
        }

        /// <summary>
        /// Adds a new entity and assigns it a unique identifier.
        /// </summary>
        /// <group>Entity V1</group>
        /// <verb>POST</verb>
        /// <url>http://localhost/entities</url>
        /// <header name="MS-CorrelationId" type="string">Correlation ID for the request</header>
        /// <header name="MS-RequestId" type="string">Request ID for the request</header>
        /// <requestType><see cref="EntityContractV1"/></requestType>
        /// <responseType><see cref="EntityContractV1"/></responseType>
        ///
        /// <param name="entity">The entity to add</param>
        /// <returns>Created action result</returns>
        [HttpPost, Route("entities")]
        public Task<IActionResult> PostAsync([FromBody] EntityContractV1 entity)
        {
            try
            {
                entity.Id = Guid.NewGuid().ToString();

                return Task.FromResult<IActionResult>(
                    this.CreatedAtRoute("EntityControllerV1GetAsync", new { id = entity.Id }, entity));
            }
            finally
            {
                this.Request.SetEntityCreatedEvent(entity.Id);
            }
        }

        /// <summary>
        /// Retrieves a specific entity instance.
        /// </summary>
        /// <group>Entity V1</group>
        /// <verb>GET</verb>
        /// <url>http://localhost/entities/{id}</url>
        /// <header name="MS-CorrelationId" type="string">Correlation ID for the request</header>
        /// <header name="MS-RequestId" type="string">Request ID for the request</header>
        /// <responseType><see cref="EntityContractV1"/></responseType>
        ///
        /// <param name="id">The unique entity id</param>
        /// <returns>Task resulting in the entity contract</returns>
        [HttpGet, Route("entities/{id}", Name = "EntityControllerV1GetAsync")]
        public Task<EntityContractV1> GetAsync(string id)
        {
            try
            {
                var entity = new EntityContractV1
                {
                    Name = "test",
                    Blob = "blob",
                    Id = id
                };

                return Task.FromResult(entity);
            }
            finally
            {
                this.Request.SetEntityRetrievedEvent(id);
            }
        }

        /// <summary>
        /// Gets a paged collection of entities matching the query parameters.
        /// </summary>
        /// <group>Entity V1</group>
        /// <verb>GET</verb>
        /// <url>http://localhost/entities?blobSize={blobSize}</url>
        /// <header name="MS-CorrelationId" type="string">Correlation ID for the request</header>
        /// <header name="MS-RequestId" type="string">Request ID for the request</header>
        /// <responseType><see cref="PagedEntityContractV1"/></responseType>
        ///
        /// <returns>Task resulting in the paged entity set contract</returns>
        [HttpGet, Route("entities")]
        public Task<PagedEntityContractV1> GetAsync()
        {
            var pagedEntities = new PagedEntityContractV1
            {
                Items =
                {
                    new EntityContractV1
                    {
                        Blob = new string('a', 32),
                        Id = Guid.NewGuid().ToString(),
                        Name = "sample"
                    }
                },
                PagingInfo = new PagingInfoContractV1
                {
                    ContinuationToken = null,
                    TotalItems = 1
                }
            };

            return Task.FromResult(pagedEntities);
        }

        /// <summary>
        /// Updates an existing entity.
        /// </summary>
        /// <group>Entity V1</group>
        /// <verb>PUT</verb>
        /// <url>http://localhost/entities/{id}</url>
        /// <header name="MS-CorrelationId" type="string">Correlation ID for the request</header>
        /// <header name="MS-RequestId" type="string">Request ID for the request</header>
        /// <requestType><see cref="EntityContractV1"/></requestType>
        /// <responseType><see cref="EntityContractV1"/></responseType>
        ///
        /// <param name="id">The unique entity id</param>
        /// <param name="entity">The entity to update</param>
        /// <returns>Task resulting in the updated entity contract</returns>
        [HttpPut, Route("entities/{id}")]
        public Task<EntityContractV1> PutAsync(string id, [FromBody]EntityContractV1 entity)
            => id != entity.Id
                ? throw this.exceptionProvider.IdMismatchException(id, entity.Id)
                : Task.FromResult(entity);
    }
}