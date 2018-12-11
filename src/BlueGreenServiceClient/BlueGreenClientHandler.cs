//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.MarketplaceServicesCore.Core;
using Microsoft.MarketplaceServicesCore.Core.Counters;

namespace Microsoft.MarketplaceServices.ReferenceServiceClient
{
    /// <summary>
    /// Client handler used for all ReferenceService clients
    /// </summary>
    public class ReferenceClientHandler : MarketplaceClientHandler
    {
        readonly string callingServiceName;
        readonly bool ignoreSslErrors;

        /// <summary>
        /// Constructor for the client handler used by ReferenceService clients
        /// </summary>
        public ReferenceClientHandler(
            ReferenceClientSettings settings,
            ILogger<ReferenceClientHandler> logger,
            ICounterFactory counterFactory,
            ICertificateLoader certificateLoader)
                : base(
                    logger,
                    counterFactory,
                    setCorrelationVectorHeader: true,
                    logPayload: settings.LogPayloadForSuccessfulRequests)
        {
            this.callingServiceName = settings.CallingServiceName;
            this.ClientCertificateLoader = certificateLoader;
            this.ignoreSslErrors = settings.IgnoreSSLErrors;
        }

        /// <summary>
        /// Configures the HttpClientHandler used for ougoing calls
        /// </summary>
        protected override HttpClientHandler ConfigureHandler(HttpClientHandler handler)
        {
            if (this.ignoreSslErrors)
            {
                handler.ServerCertificateCustomValidationCallback
                    = (sender, certificate, chain, sslPolicyErrors) => true;
            }

            return base.ConfigureHandler(handler);
        }

        /// <summary>
        /// Name of the service making the ougoing requests
        /// </summary>
        protected override string CallingServiceName { get => this.callingServiceName; }

        /// <summary>
        /// Type of service being called
        /// </summary>
        protected override string DependencyType => "WebService";

        /// <summary>
        /// Name of servcie being called
        /// </summary>
        protected override string ExternalServiceName => "Reference";

        /// <summary>
        /// Whether to use ServiceError handling mechanisms for ougoing calls from the client
        /// </summary>
        protected override bool UseServiceError => true;

        /// <summary>
        /// Service Fault-to-Exception conversion logic
        /// </summary>
        /// <param name="fault"></param>
        /// <param name="response"></param>
        /// <returns><see cref="NotSupportedException"/> since <see cref="ServiceFault"/>s are not supported.</returns>
        protected override Exception ConvertFaultToException(ServiceFault fault, HttpResponseMessage response)
            => throw new NotSupportedException("ServiceFaults are not supported by reference client.");

        /// <summary>
        /// Conversion logic for Exceptions thrown locally, by the client
        /// </summary>
        /// <param name="clientException"></param>
        /// <returns>
        /// Either a <see cref="ReferenceClientException"/> or a <see cref="ReferenceClientTimeoutException"/>
        /// </returns>
        protected override Exception ConvertClientException(Exception clientException)
            => clientException is TaskCanceledException
                ? new ReferenceClientTimeoutException("Reference operation timed out", clientException)
                : new ReferenceClientException("An unknown error occurred", clientException);

        /// <summary>
        /// Service Error-to-Exception conversion logic
        /// </summary>
        /// <param name="serviceError"></param>
        /// <param name="response"></param>
        /// <returns>
        /// Either a <see cref="ReferenceClientServiceErrorException"/> or a <see cref="ReferenceClientException"/>
        /// </returns>
        protected override Exception ConvertServiceErrorToException(
            ServiceError serviceError,
            HttpResponseMessage response)
                => response == null
                    ? new ReferenceClientException("An unknown error occurred")
                    : new ReferenceClientServiceErrorException(response.StatusCode, serviceError);
    }
}
