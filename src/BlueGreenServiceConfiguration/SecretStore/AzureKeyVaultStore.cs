//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.MarketplaceServices.AzureKeyVault;
using Microsoft.MarketplaceServicesCore.Core;
using Microsoft.MarketplaceServicesCore.Core.Logging;

namespace Microsoft.MarketplaceServices.ReferenceServiceConfiguration.SecretStore
{
    /// <summary>
    /// Azure secret store
    /// </summary>
    public class AzureKeyVaultStore : SecretStoreBase
    {
        readonly AzureKeyVaultClient keyVaultClient = null;

        /// <summary>
        /// Instantiates new instance of <see cref="AzureKeyVaultStore"/>
        /// </summary>
        /// <param name="keyVaultClient">KeyVault Client.</param>
        public AzureKeyVaultStore(AzureKeyVaultClient keyVaultClient)
        {
            this.keyVaultClient = keyVaultClient ?? throw new ArgumentNullException(nameof(keyVaultClient));
        }

        /// <summary>
        /// Get a certificate secret from the key vault.
        /// </summary>
        /// <param name="cert">The certificate definition; <see cref="CertDefinition.CertificateFile"/>
        /// is used as the name for they key vault.</param>
        /// <returns>An <see cref="X509Certificate2"/> from the key vault.</returns>
        public async Task<X509Certificate2> GetCertificateAsync(CertDefinition cert)
            => new X509Certificate2(await this.GetSecretAsync(cert.CertificateFile).OnAnyContext());

        /// <summary>
        /// Get a secret from the key vault.
        /// </summary>
        /// <param name="secretName">The secret name.</param>
        /// <returns>The content of the secret.</returns>
        public Task<byte[]> GetSecretAsync(string secretName)
        {
            var loggingContext = new LoggingContext(
                correlationId: Guid.NewGuid(),
                requestId: Guid.NewGuid(),
                requestUri: null,
                requestMethod: null,
                contractVersion: null,
                correlationVector: null,
                scenarioHeader: null,
                users: null);

            string secretValue = this.keyVaultClient.GetSecretByNameAsync(secretName, loggingContext).Result;

            return Task.FromResult(Convert.FromBase64String(secretValue));
        }

        /// <summary>
        /// Decrypt secret to byte array
        /// </summary>
        /// <param name="key">Key of the secret</param>
        public override byte[] DecryptToByteArray(string key) => this.GetSecretAsync(key).Result;
    }
}
