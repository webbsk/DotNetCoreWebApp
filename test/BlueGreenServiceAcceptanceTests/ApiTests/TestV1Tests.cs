//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using Microsoft.MarketplaceServices.Core;
using Microsoft.MarketplaceServices.ReferenceServiceContracts.V1;
using Microsoft.MarketplaceServicesCore.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.MarketplaceServices.ReferenceServiceTests
{
    /// <summary>
    /// Tests that the service can return responses from the Test Controller V1.
    /// </summary>
    [TestClass]
    public class TestV1Tests : TestBase
    {
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
		/// Tests that the service can return a test response.
		/// </summary>
		[TestMethod]
        public async Task CanGetTestResponse()
        {
            // arrange
            int delay = 1300;
            var statusCode = HttpStatusCode.Created;

            // act
            var watch = Stopwatch.StartNew();
            TestContractV1 test = await AssemblyInit.ReferenceClientV1
                .GetTestV1Async(
                    statusCode, delay, incomingOperationName: "Mstest", context: this.LoggingContext).OnAnyContext();
            long elapsed = watch.ElapsedMilliseconds;

            // assert
            Assert.IsNotNull(test, "Expected test contract to not be null");
            Assert.AreEqual(statusCode, test.HttpStatusCode, "Status code does not match");
            Assert.AreEqual(delay, test.DelayInMilliseconds, "Delay milliseconds");
            Assert.IsTrue(elapsed > delay);
        }
    }
}