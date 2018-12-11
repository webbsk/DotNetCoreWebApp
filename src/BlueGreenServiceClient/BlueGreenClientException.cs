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
    /// Defines a reference client exception that occurs when the client contacts th remote service.
    /// </summary>
    [Serializable]
    public class ReferenceClientException : Exception
    {
        /// <summary>
        /// Instantiates new instance of <see cref="ReferenceClientException"/>
        /// </summary>
        public ReferenceClientException()
        {
        }

        /// <summary>
        /// Instantiates new instance of <see cref="ReferenceClientException"/>
        /// </summary>
        /// <param name="message">Error message.</param>
        public ReferenceClientException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Instantiates new instance of <see cref="ReferenceClientException"/>
        /// </summary>
        /// <param name="innerException">Inner exception.</param>
        /// <param name="message">Error message.</param>
        public ReferenceClientException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Instantiates new instance of <see cref="ReferenceClientException"/>
        /// </summary>
        protected ReferenceClientException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
