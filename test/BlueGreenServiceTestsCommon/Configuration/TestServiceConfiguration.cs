//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.MarketplaceServices.ReferenceServiceConfiguration.Service;
using Microsoft.MarketplaceServicesCore.Authentication.Aad;
using Microsoft.MarketplaceServicesCore.Core;
using Microsoft.MarketplaceServicesCore.ServiceCore;
using Microsoft.MarketplaceServicesCore.ServiceCore.Authentication.Certificate;

namespace Microsoft.MarketplaceServices.ReferenceServiceTestsCommon.Configuration
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class TestServiceConfiguration : ServiceConfigurationBase
    {
        public const string TrustedObjectId = "f343e95b-1c83-452d-a372-106b6b3175ac";
        public const string TrustedTenantId = "124edf19-b350-4797-aefc-3206115ffdb3";
        public const string TrustedApplicationId = "a63e7d60-892a-44a6-95e6-0a9574b0cd15";
        public const string TrustedPartnerId = "RefSvc-Client-SPN";

        public override AadAuthenticationSettings AadAuthenticationSettings
        {
            get
            {
                var setting = new AadAuthenticationSettings()
                {
                    AllowNonSecureTraffic = true,
                    FederationMetadataCacheSettings = new FederationMetadataCacheSettings()
                    {
                        CacheTime = TimeSpan.FromSeconds(1),
                        FederationMetadataClient = new FederationMetadataClient()
                    },
                    // Use a hard coded file for the Federation Metadata which drives the token validation
                    FederationXmlSource = @"Configuration\TestFederationMetadata.xml",
                    TurnoffSignatureVerificationForMetadata = true,
                };
                setting.Audiences.Add("http://audience");

                // Trusted AAD App registrations.
                var acl = new PartnerAccessControlList<string>(StringComparer.OrdinalIgnoreCase);
                acl.AddAadTrustedPartner(
                    new AadTrustedPartner(
                        applicationId: TrustedApplicationId,
                        tenantId: TrustedTenantId,
                        partnerId: TrustedPartnerId));

                setting.TrustedPartnerMap.TryAddProfile(
                    profile: "Default",
                    accessControlList: acl);

                return setting;
            }
        }

        public override CommonNameAccessControlMap CommonNameAccessControlMap
        {
            get
            {
                PartnerAccessControlList<string>
                   certAccessControl = new PartnerAccessControlList<string>(StringComparer.OrdinalIgnoreCase)
                   {
                        {"myname","test partner" }
                   };

                CommonNameAccessControlMap
                    aclProfile = new CommonNameAccessControlMap()
                    {
                        {"Default",certAccessControl }
                    };

                return aclProfile;
            }
        }

        public override Uri KeyVaultBaseUri => throw new InvalidOperationException("Notimplemented");

        public override long MaxRequestBodySize => 10485760;

        public override string HttpPrefix => "http://localhost:8080/";

        public override string HttpsPrefix => "https://localhost:8086/";

        public override Guid HttpsApplicationId => new Guid("40d9d373-8927-4789-a80c-bd8b7d641f08");

        public override CertDefinition HttpsCertificate => new CertDefinition
        {
            CertificateFile = "testAad.pfx",
            CommonName = "myName",
            CertStoreLocation = StoreLocation.LocalMachine,
            CertStoreName = StoreName.My,
            SecureShare = @".\AadCert",
            Thumbprint = "751946EE3482399B425BC93E926559E427CC4C46"
        };

        public override string MdmAccountName => throw new InvalidOperationException("Notimplemented");

        public override string MdmNameSpace => throw new InvalidOperationException("Notimplemented");

        public override ThrottleConfiguration ThrottleConfiguration =>
            new ThrottleConfiguration
            {
                MaxAccepts = 10,
                MaxConnections = 10,
                RequestQueueLimit = 2000,
                RequestQueueTimeout = TimeSpan.FromSeconds(30)
            };

        public override TimeSpan VaultTimeOut => TimeSpan.FromSeconds(5);
    }
}
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
