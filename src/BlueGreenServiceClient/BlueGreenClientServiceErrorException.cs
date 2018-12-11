//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System;
using System.Net;
using System.Runtime.Serialization;
using Microsoft.MarketplaceServicesCore.Core;

namespace Microsoft.MarketplaceServices.ReferenceServiceClient
{
    /// <summary>
    /// A reference client service error exception; occurs when a remote service returns a valid error code.
    /// </summary>
    [Serializable]
    public class ReferenceClientServiceErrorException : ReferenceClientException
    {
        /// <summary>
        /// Gets the ServiceError indicating the cause of failure
        /// </summary>
        public ServiceError ServiceError { get => this.serviceError; }

        [NonSerialized]
        readonly ServiceError serviceError;

        /// <summary>
        /// The status code returned by the service.
        /// </summary>
        public HttpStatusCode StatusCode { get => this.statusCode; }

        [NonSerialized]
        readonly HttpStatusCode statusCode;

        /// <summary>
        /// Instantiates new instance of <see cref="ReferenceClientServiceErrorException"/>
        /// </summary>
        public ReferenceClientServiceErrorException()
        {
        }

        /// <summary>
        /// Instantiates new instance of <see cref="ReferenceClientServiceErrorException"/>
        /// </summary>
        /// <param name="message">Error mesage</param>
        public ReferenceClientServiceErrorException(string message) : base(message)
        {
        }

        /// <summary>
        /// Instantiates new instance of <see cref="ReferenceClientServiceErrorException"/>
        /// </summary>
        /// <param name="innerException">Inner Exception</param>
        /// <param name="message">Error message</param>
        public ReferenceClientServiceErrorException(
            string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Instantiates new instance of <see cref="ReferenceClientServiceErrorException"/>
        /// </summary>
        /// <param name="serviceError">Service error from the reference service</param>
        /// <param name="statusCode">Status code from the reference service.</param>
        public ReferenceClientServiceErrorException(HttpStatusCode statusCode, ServiceError serviceError)
            : base(serviceError == null ? "Unknown error" : serviceError.ToString())
        {
            this.serviceError = serviceError;
            this.statusCode = statusCode;
        }

        /// <summary>
        /// Instantiates new instance of <see cref="ReferenceClientServiceErrorException"/>
        /// </summary>
        protected ReferenceClientServiceErrorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            this.serviceError = (ServiceError)info.GetValue(nameof(this.ServiceError), typeof(ServiceError));
            this.statusCode = (HttpStatusCode)info.GetValue(nameof(this.StatusCode), typeof(HttpStatusCode));
        }

        /// <summary>
        /// Instantiates new instance of <see cref="ReferenceClientServiceErrorException"/>
        /// </summary>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(this.ServiceError), this.serviceError);
            info.AddValue(nameof(this.StatusCode), this.statusCode);
        }
    }
}
