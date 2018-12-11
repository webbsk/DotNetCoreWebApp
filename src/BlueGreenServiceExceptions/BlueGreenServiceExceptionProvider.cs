//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System;
using System.Net;
using Microsoft.MarketplaceServicesCore.Core;

namespace Microsoft.MarketplaceServices.ReferenceServiceExceptions
{
    /// <summary>
    /// Exception Provider class for the service.
    /// </summary>
    public class ReferenceServiceExceptionProvider : ServiceErrorExceptionProvider
    {
        /// <summary>
        /// Error source for the service.
        /// </summary>
        public const string ErrorSource = "ReferenceFD";

        /// <summary>
        /// Instantiates a new instance of <see cref="ReferenceServiceExceptionProvider"/>
        /// </summary>
        public ReferenceServiceExceptionProvider() : base(source: ErrorSource)
        {
        }

        /// <summary errorCategory="BadRequest" httpStatusCode="400">
        /// We attemped to update a entry in the receipt table that is already processed
        /// </summary>
        public const string IdMismatch = "IdMismatch";

        /// <summary errorCategory="Unauthorized" httpStatusCode="401">
        /// Request was not authorized.
        /// </summary>
        public const string ClaimMissing = "ClaimMissing";

        /// <summary>
        /// Returns an exception wrapping a error response indicating that the URI ID and the request body ID differ.
        /// </summary>
        /// <param name="pathArgumentId">ID from the URI path argument</param>
        /// <param name="requestId">ID from the request body</param>
        /// <returns>Exception wrapping the error response</returns>
        public Exception IdMismatchException(string pathArgumentId, string requestId)
        {
            var serviceError = new ServiceError(
                code: IdMismatch,
                message: "The ID value present in the URI differs from the one present in the request body.")
            {
                Source = Source
            };

            serviceError.Data.Add(pathArgumentId);
            serviceError.Data.Add(requestId);

            return this.ToException(HttpStatusCode.BadRequest, serviceError);
        }

        /// <summary>
        /// Returns an exception wrapping a error response indicating that request was not authorized due to
        /// missing claims.
        /// </summary>
        /// <returns>Exception wrapping the error response</returns>
        public Exception ClaimMissingException(string claimType)
        {
            var serviceError = new ServiceError(
                code: ClaimMissing,
                message: "A required claim is missing")
            {
                Source = Source
            };

            serviceError.Data.Add(claimType);

            return this.ToException(HttpStatusCode.Unauthorized, serviceError);
        }

        /// <summary>
        /// Converts error from downstream error.
        /// </summary>
        /// <param name="statusCode">HttpStatus code</param>
        /// <param name="serviceError">Service Error</param>
        /// <returns>Returns a custom HttpResponseException</returns>
        protected override Exception ToExceptionCore(HttpStatusCode statusCode, ServiceError serviceError)
            => new ServiceErrorException(statusCode, serviceError);
    }
}
