//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.MarketplaceServices.ReferenceServiceClient;
using Microsoft.MarketplaceServices.ReferenceServiceClient.V1;
using Microsoft.MarketplaceServices.ReferenceServiceContracts.V1;
using Microsoft.MarketplaceServices.ReferenceServiceEvents.FDEvents;
using Microsoft.MarketplaceServices.ReferenceServiceExceptions;
using Microsoft.MarketplaceServices.ReferenceServiceTestsCommon;
using Microsoft.MarketplaceServices.ReferenceServiceTestsCommon.Configuration;
using Microsoft.MarketplaceServicesCore.Authentication;
using Microsoft.MarketplaceServicesCore.Authentication.Aad;
using Microsoft.MarketplaceServicesCore.Core;
using Microsoft.MarketplaceServicesCore.Core.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.MarketplaceServices.ReferenceServiceTests
{
    /// <summary>
    /// Tests that the service can perform CRUD operations on V1 entities.
    /// </summary>
    [TestClass]
    public class EntityV1Tests : TestBase
    {
        readonly string scenarioName = "EntityV1Test";

        /// <summary>
        /// Test initialize method.
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
        /// Tests that the service can create an entity.
        /// </summary>
        [TestMethod]
        public async Task AddEntityV1AuthorizationHeaderOnly()
        {
            var referenceClientSettings = new ReferenceClientSettings
            {
                BaseAddress = new Uri(AssemblyInit.EnvironmentRuntime.ServiceConfiguration.HttpsPrefix),
                CallingServiceName = "Test",
                Timeout = Debugger.IsAttached ? TimeSpan.FromDays(7) : TimeSpan.FromSeconds(30),
            };

            var referenceClientV1 = new ReferenceClientV1(
                AssemblyInit.ReferenceClientSettings,
                new ReferenceClientHandler(
                    AssemblyInit.ReferenceClientSettings,
                    this.LoggerFactory.CreateLogger<ReferenceClientHandler>(),
                    AssemblyInit.CounterFactory,
                    certificateLoader: null));

            string issuer = "testissuer";

            // Create Token
            var claims = new List<Claim>()
            {
                new Claim(AuthenticationClaimTypes.AppId, TestServiceConfiguration.TrustedApplicationId),
                new Claim(AadClaimTypes.ObjectId, TestServiceConfiguration.TrustedObjectId),
                new Claim(AadClaimTypes.TenantId, TestServiceConfiguration.TrustedTenantId),
            };

            JwtSecurityToken token = CreateSignedToken(issuer, "http://audience", claims);
            string authHeader = $"Bearer {new JwtSecurityTokenHandler().WriteToken(token)}";

            using (var listener = new MemoryEventListener())
            {
                var entity = new EntityContractV1
                {
                    Id = "6",
                    Name = "abc",
                    Blob = Convert.ToBase64String(new byte[] { 1, 2, 3, 4 }),
                    Count = 5,
                    UserId = 1
                };

                // act
                EntityContractV1 newEntity = await referenceClientV1.AddEntityV1Async(
                    entity,
                    this.scenarioName,
                    authHeader,
                    this.LoggingContext).OnAnyContext();

                entity.Id = newEntity.Id;

                // assert
                Assert.IsNotNull(newEntity);
                Assert.That.EntityV1Equals(expectedEntity: entity, actualEntity: newEntity);

                Assert.IsTrue(listener.LoggedEvents.Count > 0, "Expected something to be logged");

                LoggedEventData loggedEvent = listener.FindEvents(typeof(EntityCreatedEvent).FullName)[0];
                Assert.IsNotNull(loggedEvent, "Expected a EntityCreatedEvent to be logged");
                Assert.AreEqual(newEntity.Id, loggedEvent.GetLoggedPath<string>("data.EntityId"));
            }
        }

        /// <summary>
        /// Test which verifies HTTP 401 from service when no authorization header/certificate is passed.
        /// </summary>
        [TestMethod]
        public async Task AddEntityV1NoAuth()
        {
            string authHeader = null;

            var referenceClientSettings = new ReferenceClientSettings
            {
                BaseAddress = new Uri(AssemblyInit.EnvironmentRuntime.ServiceConfiguration.HttpsPrefix),
                CallingServiceName = "Test",
                Timeout = Debugger.IsAttached ? TimeSpan.FromDays(7) : TimeSpan.FromSeconds(30),
            };

            var referenceClientV1 = new ReferenceClientV1(
                AssemblyInit.ReferenceClientSettings,
                new ReferenceClientHandler(
                    AssemblyInit.ReferenceClientSettings,
                    this.LoggerFactory.CreateLogger<ReferenceClientHandler>(),
                    AssemblyInit.CounterFactory,
                    certificateLoader: null));

            using (var listener = new MemoryEventListener())
            {
                var entity = new EntityContractV1
                {
                    Id = "6",
                    Name = "abc",
                    Blob = Convert.ToBase64String(new byte[] { 1, 2, 3, 4 }),
                    Count = 5,
                    UserId = 1
                };

                var request = new Func<Task>(
                    () => referenceClientV1.AddEntityV1Async(
                        entity,
                        this.scenarioName,
                        authHeader,
                        this.LoggingContext));

                await Assert.That.RequestReturnsServiceErrorAsync(
                    request,
                    HttpStatusCode.Unauthorized,
                    ReferenceServiceExceptionProvider.ClaimMissing,
                    "A required claim is missing").OnAnyContext();
            }
        }

        /// <summary>
        /// Test which verifies success when certificate is passed.
        /// </summary>
        /// <returns></returns>
        [TestMethod]
        public async Task AddEntityV1CertificateAuthOnly()
        {
            var referenceClientSettings = new ReferenceClientSettings
            {
                BaseAddress = new Uri(AssemblyInit.EnvironmentRuntime.ServiceConfiguration.HttpsPrefix),
                CallingServiceName = "Test",
                Timeout = Debugger.IsAttached ? TimeSpan.FromDays(7) : TimeSpan.FromSeconds(30),
            };

            var referenceClientV1 = new ReferenceClientV1(
                AssemblyInit.ReferenceClientSettings,
                new ReferenceClientHandler(
                    AssemblyInit.ReferenceClientSettings,
                    this.LoggerFactory.CreateLogger<ReferenceClientHandler>(),
                    AssemblyInit.CounterFactory,
                    certificateLoader: new X509StoreCertificateLoader(
                        AssemblyInit.EnvironmentRuntime.ServiceConfiguration.HttpsCertificate)));

            var entity = new EntityContractV1
            {
                Id = "6",
                Name = "abc",
                Blob = Convert.ToBase64String(new byte[] { 1, 2, 3, 4 }),
                Count = 5,
                UserId = 1
            };

            // act
            EntityContractV1 newEntity = await referenceClientV1.AddEntityV1Async(
                entity,
                this.scenarioName,
                authHeader: null,
                context: this.LoggingContext).OnAnyContext();

            entity.Id = newEntity.Id;

            // assert
            Assert.IsNotNull(newEntity);
            Assert.That.EntityV1Equals(expectedEntity: entity, actualEntity: newEntity);
        }

        static EntityContractV1 BuildEntity(string id = null)
        {
            return new EntityContractV1
            {
                Blob = "blob",
                Count = 1234,
                Id = id,
                Name = "name"
            };
        }

        /// <summary>
        /// Tests that the service can return an entity by an id.
        /// </summary>
        [TestMethod]
        public async Task CanGetEntityV1ById()
        {
            string entityId = "1";

            using (var listener = new MemoryEventListener())
            {
                // act
                EntityContractV1 entity = await AssemblyInit.ReferenceClientV1.GetEntityV1Async(
                    entityId,
                    this.scenarioName,
                    this.LoggingContext).OnAnyContext();

                // assert
                Assert.IsNotNull(entity);
                Assert.AreEqual(entityId, entity.Id);

                Assert.IsTrue(listener.LoggedEvents.Count > 0, "Expected something to be logged");

                LoggedEventData loggedEvent = listener.FindEvents(typeof(EntityRetrievedEvent).FullName)[0];
                Assert.IsNotNull(loggedEvent, "Expected a EntityCreatedEvent to be logged");
                Assert.AreEqual(entity.Id, loggedEvent.GetLoggedPath<string>("data.EntityId"));
            }
        }

        /// <summary>
        /// Tests that hte service can get entities by their user id.
        /// </summary>
        [TestMethod]
        public async Task CanGetEntitiesV1ByUserIdUsingPagination()
        {
            // act
            var items = new List<EntityContractV1>();
            string continuationToken = null;
            do
            {
                PagedEntityContractV1 pagedResponse =
                    await AssemblyInit.ReferenceClientV1.GetEntitiesV1Async(
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
                () => AssemblyInit.ReferenceClientV1.UpdateEntityV1Async(
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