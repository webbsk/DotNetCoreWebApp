//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.MarketplaceServices.ReferenceServiceContracts.V2;
using Microsoft.MarketplaceServices.ReferenceServiceEvents.FDEvents;
using Microsoft.MarketplaceServices.ReferenceServiceExceptions;
using Microsoft.MarketplaceServicesCore.Core.Counters;

namespace Microsoft.MarketplaceServices.ReferenceServiceControllers.V2
{
    /// <summary>
    /// Defines V2 operations for the entities resource.
    /// </summary>
    [Route("v2.0")]
    public sealed class EntityControllerV2 : ApiController
    {
        readonly ICounterFactory counterfactory;
        readonly ReferenceServiceExceptionProvider exceptionProvider;

        /// <summary>
        /// Creates the default entity controller.
        /// </summary>
        /// <param name="counterfactory">Counterfactory to use.</param>
        /// <param name="exceptionProvider">Error Provider.</param>
        public EntityControllerV2(
            ICounterFactory counterfactory,
            ReferenceServiceExceptionProvider exceptionProvider)
        {
            this.counterfactory = counterfactory ?? throw new ArgumentNullException(nameof(counterfactory));
            this.exceptionProvider = exceptionProvider ?? throw new ArgumentNullException(nameof(exceptionProvider));
        }

        /// <summary>
        /// Adds a new entity and assigns it a unique identifier.
        /// </summary>
        /// <group>Entity V2</group>
        /// <verb>POST</verb>
        /// <url>http://localhost/entities</url>
        /// <header name="MS-Contract-Version" type="number">Specifies the verions of the contracts used</header>
        /// <header name="MS-CorrelationId" type="string">Correlation ID for the request</header>
        /// <header name="MS-RequestId" type="string">Request ID for the request</header>
        /// <requestType><see cref="EntityContractV2"/></requestType>
        /// <responseType><see cref="EntityContractV2"/></responseType>
        ///
        /// <param name="entity">The entity to add</param>
        /// <returns>Created action result</returns>
        [HttpPost, Route("entities")]
        public Task<IActionResult> PostAsync([FromBody] EntityContractV2 entity)
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
        /// <group>Entity V2</group>
        /// <verb>GET</verb>
        /// <url>http://localhost/entities/{id}</url>
        /// <header name="MS-Contract-Version" type="number">Specifies the verions of the contracts used</header>
        /// <header name="MS-CorrelationId" type="string">Correlation ID for the request</header>
        /// <header name="MS-RequestId" type="string">Request ID for the request</header>
        /// <responseType><see cref="EntityContractV2"/></responseType>
        /// <param name="id">The unique entity id</param>
        /// <returns>Task resulting in the entity contract</returns>
        [HttpGet, Route("entities/{id}", Name = "EntityControllerV2GetAsync")]
        public Task<EntityContractV2> GetAsync(string id)
        {
            try
            {
                var entity = new EntityContractV2
                {
                    Blob = "blob",
                    Id = id,
                    Number = 10
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
        /// <group>Entity V2</group>
        /// <verb>GET</verb>
        /// <url>http://localhost/entities?blobSize={blobSize}</url>
        /// <header name="MS-Contract-Version" type="number">Specifies the verions of the contracts used</header>
        /// <header name="MS-CorrelationId" type="string">Correlation ID for the request</header>
        /// <header name="MS-RequestId" type="string">Request ID for the request</header>
        /// <responseType><see cref="PagedEntityContractV2"/></responseType>
        ///
        /// <returns>Task resulting in the paged entity set contract</returns>
        [HttpGet, Route("entities")]
        public Task<PagedEntityContractV2> GetAsync()
        {
            var pagedEntities = new PagedEntityContractV2()
            {
                Items =
                {
                    new EntityContractV2
                    {
                        Blob = new string('a', 32),
                        Id = Guid.NewGuid().ToString(),
                        Number = 32
                    }
                },
                PagingInfo = new PagingInfoContractV2
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
        /// <group>Entity V2</group>
        /// <verb>PUT</verb>
        /// <url>http://localhost/entities/{id}</url>
        /// <header name="MS-Contract-Version" type="number">Specifies the verions of the contracts used</header>
        /// <header name="MS-CorrelationId" type="string">Correlation ID for the request</header>
        /// <header name="MS-RequestId" type="string">Request ID for the request</header>
        /// <requestType><see cref="EntityContractV2"/></requestType>
        /// <responseType><see cref="EntityContractV2"/></responseType>
        /// <param name="id">The unique entity id</param>
        /// <param name="entity">The entity to update</param>
        /// <returns>Task resulting in the updated entity contract</returns>
        [HttpPut, Route("entities/{id}")]
        public Task<EntityContractV2> PutAsync(string id, [FromBody]EntityContractV2 entity)
            => id != entity.Id
                ? throw this.exceptionProvider.IdMismatchException(id, entity.Id)
                : Task.FromResult(entity);
    }
}