//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System.Security.Cryptography.X509Certificates;
using Microsoft.MarketplaceServices.Core;
using Microsoft.MarketplaceServicesCore.Core;

namespace Microsoft.MarketplaceServices.ReferenceServiceTests
{
    /// <summary>
    /// Defines a test certificate loader.
    /// </summary>
    public class TestCertificateLoader : ICertificateLoader
	{
		/// <summary>
		/// Loads an empty certifiate.
		/// </summary>
		/// <returns>Null.</returns>
		public X509Certificate2 LoadCertificate() => null;
	}
}