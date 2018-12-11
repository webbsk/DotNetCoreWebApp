//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.MarketplaceServices.ReferenceServiceContracts.V2;
using Microsoft.MarketplaceServices.ReferenceServiceExceptions;
using Microsoft.MarketplaceServices.ReferenceServiceTestsCommon;
using Microsoft.MarketplaceServicesCore.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.MarketplaceServices.ReferenceServiceTests
{
    /// <summary>
    /// Tests that the service can perform CRUD operations against V2 entity controllers.
    /// </summary>
    [TestClass]
    public class EntityV2Tests : TestBase
    {
        readonly string scenarioName = "EntityV2Test";

        /// <summary>
        /// Test initialize
        /// </summary>
        [TestInitialize]
        public void TestInitialize()
        {
            base.DoInitialize();
        }

        /// <summary>
        /// Test cleanup
        /// </summary>
        [TestCleanup]
        public void TestCleanup()
        {
            base.DoCleanup();
        }

        /// <summary>
        /// Tests that the service can add an entity.
        /// </summary>
        [TestMethod]
        public async Task AddEntityV2()
        {
            var entity = new EntityContractV2
            {
                Id = "6",
                Blob = Convert.ToBase64String(new byte[] { 1, 2, 3, 4 }),
                Number = 5,
                UserId = 1
            };

            // act
            EntityContractV2 newEntity = await AssemblyInit.ReferenceClientV2.AddEntityV2Async(
                entity,
                this.scenarioName,
                this.LoggingContext).OnAnyContext();

            entity.Id = newEntity.Id;

            // assert
            Assert.IsNotNull(newEntity);
            Assert.That.EntityV2Equals(expectedEntity: entity, actualEntity: newEntity);
        }

        static EntityContractV2 BuildEntity(string id = null)
        {
            return new EntityContractV2
            {
                Blob = "blob",
                Id = id,
            };
        }

        /// <summary>
        /// Tests that the service can return an entity by an id.
        /// </summary>
        [TestMethod]
        public async Task CanGetEntityV2ById()
        {
            string entityId = "1";

            // act
            EntityContractV2 entity = await AssemblyInit.ReferenceClientV2.GetEntityV2Async(
                entityId,
                this.scenarioName,
                this.LoggingContext).OnAnyContext();

            // assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(entityId, entity.Id);
        }

        /// <summary>
        /// Tests that the service can return entities by user id.
        /// </summary>
        [TestMethod]
        public async Task CanGetEntitiesV2ByUserIdUsingPagination()
        {
            // act
            var items = new List<EntityContractV2>();
            string continuationToken = null;

            do
            {
                PagedEntityContractV2 pagedResponse = await AssemblyInit.ReferenceClientV2.GetEntitiesV2Async(
                    userId: 1,
                    continuationToken: continuationToken,
                    incomingOperationName: this.scenarioName,
                    context: this.LoggingContext).OnAnyContext();

                items.AddRange(pagedResponse.Items);
                continuationToken = pagedResponse.Items.Count == 0 ? null : pagedResponse.PagingInfo.ContinuationToken;
            } while (continuationToken != null);

            // assert
            Assert.AreEqual(1, items.Count, "Expected one entry");
        }

        /// <summary>
        /// Tests whether mistmached identity exception is thrown.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task UpdateEntityFaultsForMismatchedIds()
        {
            var request = new Func<Task>(
                () => AssemblyInit.ReferenceClientV2.UpdateEntityV2Async(
                    id: "123",
                    entity: BuildEntity("abc"),
                    incomingOperationName: this.scenarioName,
                    context: this.LoggingContext));

            await Assert.That.RequestReturnsServiceErrorAsync(
                request,
                HttpStatusCode.BadRequest,
                ReferenceServiceExceptionProvider.IdMismatch,
                innerErrorMessage: "The ID value present in the URI differs from the one present in the request body.")
                    .ConfigureAwait(continueOnCapturedContext: false);
        }
    }
}