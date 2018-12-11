//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.MarketplaceServicesCore.Core;
using Microsoft.MarketplaceServicesCore.Core.Logging;
using Newtonsoft.Json;

namespace Microsoft.MarketplaceServices.ReferenceServiceClient
{
    /// <summary>
    /// Abstract implementation of a generic reference Client
    /// </summary>
    public abstract class ReferenceClientBase : IDisposable
    {
        /// <summary>
        /// The application/json media type.
        /// </summary>
        public const string JsonMediaType = "application/json";

        /// <summary>
        /// The supported contract version for this client.
        /// </summary>
        protected abstract Version ContractVersion { get; }

        /// <summary>
        /// The underlying HTTP client.
        /// </summary>
        protected HttpClient Client { get; set; }

        /// <summary>
        /// Create a new reference base client.
        /// </summary>
        /// <param name="settings">The remote service settings.</param>
        /// <param name="handler">Reference Client Handler.</param>
        protected ReferenceClientBase(ReferenceClientSettings settings, ReferenceClientHandler handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            this.Client = new HttpClient(handler, disposeHandler: true)
            {
                BaseAddress = settings.BaseAddress,
                MaxResponseContentBufferSize = 10 * 1024 * 1024, // 10MB
                Timeout = settings.Timeout
            };

            this.Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(JsonMediaType));
        }

        /// <summary>
        /// Generates the dependency logging data.
        /// </summary>
        /// <param name="incomingOperatioName">Incomign operation name.</param>
        /// <param name="operationName">Operation Name.</param>
        /// <param name="operationVersion">Version</param>
        /// <returns></returns>
        protected DependencyRequestLoggingData GetDependencyRequestLoggingData(
           string incomingOperatioName,
           string operationName,
           string operationVersion)
        {
            return new DependencyRequestLoggingData
            {
                IncomingOperationName = incomingOperatioName,
                OperationName = operationName,
                OperationVersion = operationVersion,
                RequestId = Guid.NewGuid().ToString()
            };
        }

        /// <summary>
        /// Post a JSON request.
        /// </summary>
        /// <typeparam name="TResult">The expected response type.</typeparam>
        /// <param name="path">The relative path.</param>
        /// <param name="counter">The counter category.</param>
        /// <param name="content">The request body.</param>
        /// <param name="context">The request context.</param>
        /// <param name="authHeader">Authorization Header.</param>
        /// <returns>A service result.</returns>
        protected async Task<TResult> PostAsync<TResult>(
            string path,
            DependencyRequestLoggingData counter,
            object content,
            LoggingContext context,
            string authHeader = null)
                where TResult : class =>
                    await this.SendAsync<TResult>(
                        path,
                        counter,
                        context,
                        HttpMethod.Post,
                        content,
                        authHeader).OnAnyContext();

        /// <summary>
        /// Put a JSON request.
        /// </summary>
        /// <typeparam name="TResult">The expected response type.</typeparam>
        /// <param name="path">The relative path.</param>
        /// <param name="counter">The counter category.</param>
        /// <param name="content">The request body.</param>
        /// <param name="context">The request context.</param>
        /// <returns>A service result.</returns>
        protected async Task<TResult> PutAsync<TResult>(
            string path,
            DependencyRequestLoggingData counter,
            object content,
            LoggingContext context)
                where TResult : class =>
                    await this.SendAsync<TResult>(path, counter, context, HttpMethod.Put, content).OnAnyContext();

        /// <summary>
        /// Get a JSON request.
        /// </summary>
        /// <typeparam name="TResult">The expected response type.</typeparam>
        /// <param name="path">The relative path.</param>
        /// <param name="counter">The counter category.</param>
        /// <param name="context">The request context.</param>
        /// <returns>A service result.</returns>
        protected async Task<TResult> GetAsync<TResult>(
            string path,
            DependencyRequestLoggingData counter,
            LoggingContext context)
                where TResult : class =>
                    await this.SendAsync<TResult>(path, counter, context, HttpMethod.Get).OnAnyContext();

        /// <summary>
        /// Send a JSON request.
        /// </summary>
        /// <typeparam name="TResult">The expected response type.</typeparam>
        /// <param name="path">The relative path.</param>
        /// <param name="dependencyRequestLoggingData">Depedency request</param>
        /// <param name="content">The request body.</param>
        /// <param name="context">The request context.</param>
        /// <param name="method">The http method.</param>
        /// <param name="authHeader">The Authentication Header.</param>
        /// <returns>A service result.</returns>
        protected async Task<TResult> SendAsync<TResult>(
            string path,
            DependencyRequestLoggingData dependencyRequestLoggingData,
            LoggingContext context,
            HttpMethod method = null,
            object content = null,
            string authHeader = null) where TResult : class
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var versionedRelativePath = new Uri($"v{this.ContractVersion}/{path}", UriKind.Relative);
            using (var request = new HttpRequestMessage(method ?? HttpMethod.Get, versionedRelativePath))
            {
                if (content != null)
                {
                    request.Content = new StringContent(
                        JsonConvert.SerializeObject(content),
                        Encoding.UTF8,
                        JsonMediaType);
                }

                if (!String.IsNullOrWhiteSpace(authHeader))
                {
                    request.Headers.Add("Authorization", authHeader);
                }

                using (HttpResponseMessage response = await this.Client.SendAsync(
                    request,
                    context,
                    dependencyRequestLoggingData,
                    CancellationToken.None).OnAnyContext())
                {
                    return await response.Content.ReadAsAsync<TResult>().OnAnyContext();
                }
            }
        }

        /// <summary>
        /// Dispose of the client and underlying resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose of the client and underlying resources.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Client.Dispose();
            }
        }
    }
}