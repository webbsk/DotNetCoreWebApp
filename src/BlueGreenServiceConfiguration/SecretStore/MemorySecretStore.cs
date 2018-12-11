//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System.Collections.Generic;
using Microsoft.MarketplaceServicesCore.Core;

namespace Microsoft.MarketplaceServices.ReferenceServiceConfiguration.SecretStore
{
    /// <summary>
    /// Exposes environment variables and certificates as secrets.
    /// </summary>
    public class MemorySecretStore : SecretStoreBase
    {
        readonly IDictionary<string, byte[]> secrets = null;

        public MemorySecretStore(IDictionary<string, byte[]> secrets = null)
        {
            this.secrets = secrets ?? new Dictionary<string, byte[]>();
        }

        public override byte[] DecryptToByteArray(string key)
            => this.secrets.TryGetValue(key, out byte[] val) ? val : null;
    }
}