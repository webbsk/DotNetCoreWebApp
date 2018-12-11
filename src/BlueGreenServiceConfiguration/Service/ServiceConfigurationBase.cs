//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.MarketplaceServicesCore.Authentication.Aad;
using Microsoft.MarketplaceServicesCore.Core;
using Microsoft.MarketplaceServicesCore.ServiceCore.Authentication.Certificate;

namespace Microsoft.MarketplaceServices.ReferenceServiceConfiguration.Service
{
    /// <summary>
    /// Defines the configuration for this service.
    /// </summary>
    public abstract class ServiceConfigurationBase
    {
        /// <summary>
        /// Gets the Azure Active Directory settings for reference service.
        /// </summary>
        public abstract AadAuthenticationSettings AadAuthenticationSettings { get; }

        /// <summary>
        /// Gets the certificate profile mapping for common name based cert validation.
        /// </summary>
        public abstract CommonNameAccessControlMap CommonNameAccessControlMap { get; }

        /// <summary>
        /// Base Uri for KeyVault.
        /// </summary>
        public abstract Uri KeyVaultBaseUri { get; }

        /// <summary>
        /// Maximum request body size.
        /// </summary>
        public abstract long MaxRequestBodySize { get; }

        /// <summary>
        /// Msi Base Uri.
        /// </summary>
        public virtual Uri MsiBaseUri => new Uri("http://169.254.169.254/");

        /// <summary>
        /// The http prefix.
        /// </summary>
        public abstract string HttpPrefix { get; }

        /// <summary>
        /// The https prefix.
        /// </summary>
        public abstract string HttpsPrefix { get; }

        /// <summary>
        /// The https application id.
        /// </summary>
        public abstract Guid HttpsApplicationId { get; }

        /// <summary>
        /// The https certificate.
        /// </summary>
        public abstract CertDefinition HttpsCertificate { get; }

        /// <summary>
        /// Mdm account name
        /// </summary>
        public abstract string MdmAccountName { get; }

        /// <summary>
        /// Mdm namespace
        /// </summary>
        public abstract string MdmNameSpace { get; }

        /// <summary>
        /// The certificates to be installed.
        /// </summary>
        public IEnumerable<CertDefinition> CertificatesToInstall => new[] { this.HttpsCertificate };

        /// <summary>
        /// Throttle settings for service.
        /// </summary>
        public abstract ThrottleConfiguration ThrottleConfiguration { get; }

        /// <summary>
        /// Timeout connecting to Azure Keyvault.
        /// </summary>
        public abstract TimeSpan VaultTimeOut { get; }

        /// <summary>
        /// All the listening prefixes.
        /// </summary>
        public IEnumerable<string> AllPrefixes
            => new[] { this.HttpPrefix, this.HttpsPrefix }.Where(x => !string.IsNullOrWhiteSpace(x));
    }
}