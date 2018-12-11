//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System;
using System.Runtime.Serialization;

namespace Microsoft.MarketplaceServices.ReferenceServiceClient
{
    /// <summary>
    /// Reference client timeout exception; occurs when the remote service times out on a request.
    /// </summary>
    [Serializable]
    public class ReferenceClientTimeoutException : ReferenceClientException
    {
        /// <summary>
        /// Instantiates new instance <see cref="ReferenceClientTimeoutException"/>
        /// </summary>
        public ReferenceClientTimeoutException()
        {
        }

        /// <summary>
        /// Instantiates new instance <see cref="ReferenceClientTimeoutException"/>
        /// </summary>
        /// <param name="message">Error message.</param>
        public ReferenceClientTimeoutException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Instantiates new instance <see cref="ReferenceClientTimeoutException"/>
        /// </summary>
        /// <param name="innerException">Inner exception</param>
        /// <param name="message">Error Message</param>
        public ReferenceClientTimeoutException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Instantiates new instance <see cref="ReferenceClientTimeoutException"/>
        /// </summary>
        protected ReferenceClientTimeoutException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
