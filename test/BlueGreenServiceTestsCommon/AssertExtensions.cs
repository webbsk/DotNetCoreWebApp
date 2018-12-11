//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.MarketplaceServices.ReferenceServiceClient;
using Microsoft.MarketplaceServices.ReferenceServiceContracts.V1;
using Microsoft.MarketplaceServices.ReferenceServiceContracts.V2;
using Microsoft.MarketplaceServicesCore.Core;
using Microsoft.MarketplaceServicesCore.Core.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.MarketplaceServices.ReferenceServiceTestsCommon
{
    /// <summary>
    /// Class encapsulating various assertions for the test.
    /// </summary>
    public static class AssertExtensions
    {
        /// <summary>
        /// Asserts that the error has the expected code and details.  Note that the ordering of these parameters is
        /// counter to the normal assert ordering.  This is intentionally done to group the expected code with the
        /// details which must be at the end.
        /// </summary>
        /// <param name="assert">Static <see cref="Assert"/> Class</param>
        /// <param name="error">Actual ServiceError received</param>
        /// <param name="expectedCode">Expected fault code</param>
        /// <param name="expectedDetails">Expected subset or set of details (in the exact order expected)</param>
        public static void ErrorEquals(
            this Assert assert,
            ServiceError error,
            string expectedCode,
            params string[] expectedDetails)
        {
            Assert.AreEqual(expectedCode, error.InnerError.Code, "Error code unexpected");

            Assert.IsTrue(error.InnerError.Data.Count >= expectedDetails.Length, "Too few fault details");
            for (int i = 0; i < expectedDetails.Length; i++)
            {
                Assert.AreEqual(expectedDetails[i], error.InnerError.Data[i], "Detail mismatch at index {0}", i);
            }
        }

        /// <summary>
        /// Asserts the error from the service after executing the request.
        /// </summary>
        /// <param name="assert">Static <see cref="Assert"/> Class</param>
        /// <param name="request">Function to execute the request.</param>
        /// <param name="statusCode">Http Status code.</param>
        /// <param name="expectedCode">Expected error code.</param>
        /// <param name="innerErrorMessage">Expected inner message.</param>
        /// <param name="details">Expected details within the exception.</param>
        public static async Task RequestReturnsServiceErrorAsync(
            this Assert assert,
            Func<Task> request,
            HttpStatusCode statusCode,
            string expectedCode,
            string innerErrorMessage,
            params string[] details)
        {
            try
            {
                await request().OnAnyContext();
                Assert.Fail("Expected fault to be thrown from client");
            }
            catch (ReferenceClientServiceErrorException ex)
            {
                Assert.AreEqual(statusCode, ex.StatusCode, "Unexpected HTTP Status Code");
                Assert.That.ServiceErrorEquals(ex.ServiceError, expectedCode, innerErrorMessage, details);
            }
        }

        /// <summary>
        /// Asserts the error.
        /// </summary>
        /// <param name="assert">Static <see cref="Assert"/> Class</param>
        /// <param name="error">Error to assert.</param>
        /// <param name="expectedCode">Expected service error code.</param>
        /// <param name="innerErrorMessage">Expected inner error message.</param>
        /// <param name="expectedDetails">Expected exception details.</param>
        public static void ServiceErrorEquals(
            this Assert assert,
            ServiceError error,
            string expectedCode,
            string innerErrorMessage = null,
            params string[] expectedDetails)
        {
            Assert.AreEqual(expectedCode, error.Code, "Inner error code unexpected");

            if (innerErrorMessage != null)
            {
                Assert.IsNotNull(error.Message, "Unexpected innerError message");
                Assert.IsTrue(error.Message.Contains(innerErrorMessage));
            }

            if (expectedDetails != null)
            {
                Assert.IsTrue(error.Data.Count >= expectedDetails.Length, "Too few fault details");
                for (int i = 0; i < expectedDetails.Length; i++)
                {
                    Assert.AreEqual(expectedDetails[i], error.Data[i], "Detail mismatch at index {0}", i);
                }
            }
        }

        /// <summary>
        /// Validates if an SLL Events exists within logged events.
        /// </summary>
        /// <typeparam name="T">Type of event to validate.</typeparam>
        /// <param name="assert">Static <see cref="Assert"/> Class</param>
        /// <param name="listener">Event listener</param>
        /// <param name="expectedNumberOfEventsMatchingName">Number of events expected to match.</param>
        public static IEnumerable<LoggedEventData> LoggedEventsExists<T>(
            this Assert assert,
            MemoryEventListener listener,
            int expectedNumberOfEventsMatchingName = 1)
        {
            string eventName = typeof(T).FullName;

            IList<LoggedEventData> loggedEvents =
                listener.LoggedEvents.Where(x => x.EventName.Contains(eventName)).ToList();

            Assert.AreEqual(expectedNumberOfEventsMatchingName, loggedEvents.Count, "Unexpected number of events");

            return loggedEvents;
        }

        /// <summary>
        /// Validates if an SLL Events exists within logged events.
        /// </summary>
        /// <typeparam name="T">Type of event to validate.</typeparam>
        /// <param name="assert">Static <see cref="Assert"/> Class</param>
        /// <param name="listener">Event listener</param>
        /// <param name="expectedEventData">Expected event data.</param>
        /// <param name="expectedNumberOfEventsMatchingName">Number of events expected to match.</param>
        public static LoggedEventData LoggedEventExists<T>(
            this Assert assert,
            MemoryEventListener listener,
            IDictionary<string, string> expectedEventData,
            int expectedNumberOfEventsMatchingName = 1)
        {
            IEnumerable<LoggedEventData> loggedEvents = Assert.That.LoggedEventsExists<T>(
                listener,
                expectedNumberOfEventsMatchingName);

            LoggedEventData expectedEventFound =
                loggedEvents.Where(
                    loggedEvent => expectedEventData.Keys.All(
                        logEventField => String.Equals(
                            expectedEventData[logEventField],
                            loggedEvent.GetLoggedPath(logEventField),
                            StringComparison.OrdinalIgnoreCase))).FirstOrDefault();

            Assert.IsTrue(
                expectedEventFound != null,
                String.Format(CultureInfo.InvariantCulture,
                    "Could not find event of type '{0}' with expected values: {1}"
                        + "{2}{3}...in the following log events:{4}{5}",
                    typeof(T).FullName,
                    Environment.NewLine,
                    String.Join(Environment.NewLine, expectedEventData.Select(e => String.Join(":", e.Key, e.Value))),
                    Environment.NewLine,
                    Environment.NewLine,
                    String.Join(Environment.NewLine, loggedEvents)));

            return expectedEventFound;
        }

        /// <summary>
        /// Asserts equality between two instances of <see cref="EntityContractV1"/>.
        /// </summary>
        /// <param name="assert">Static <see cref="Assert"/> Class</param>
        /// <param name="expectedEntity">Expected <see cref="EntityContractV1"/></param>
        /// <param name="actualEntity">Actual  <see cref="EntityContractV1"/></param>
        public static void EntityV1Equals(
            this Assert assert,
            EntityContractV1 expectedEntity,
            EntityContractV1 actualEntity)
        {
            Assert.AreEqual(expectedEntity.Blob, actualEntity.Blob, "Expected Blob to be equal");
            Assert.AreEqual(expectedEntity.Count, actualEntity.Count, "Expected Count to be equal");
            Assert.AreEqual(expectedEntity.Id, actualEntity.Id, "Expected Id to be equal");
            Assert.AreEqual(expectedEntity.Name, actualEntity.Name, "Expected Name to be equal");
        }

        /// <summary>
        /// Asserts equality between two instances of <see cref="EntityContractV2"/>.
        /// </summary>
        /// <param name="assert">Static <see cref="Assert"/> Class</param>
        /// <param name="expectedEntity">Expected <see cref="EntityContractV2"/></param>
        /// <param name="actualEntity">Actual <see cref="EntityContractV2"/></param>>
        public static void EntityV2Equals(
            this Assert assert,
            EntityContractV2 expectedEntity,
            EntityContractV2 actualEntity)
        {
            Assert.AreEqual(expectedEntity.Blob, actualEntity.Blob, "Expected Blob to be equal");
            Assert.AreEqual(expectedEntity.Id, actualEntity.Id, "Expected Id to be equal");
            Assert.AreEqual(expectedEntity.Number, actualEntity.Number, "Expected Number to be equal");
            Assert.AreEqual(expectedEntity.UserId, actualEntity.UserId, "Expected UserId to be equal");
        }
    }
}
