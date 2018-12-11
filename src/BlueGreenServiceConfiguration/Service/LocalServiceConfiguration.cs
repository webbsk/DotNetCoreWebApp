//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.MarketplaceServicesCore.Authentication.Aad;
using Microsoft.MarketplaceServicesCore.Core;
using Microsoft.MarketplaceServicesCore.ServiceCore;
using Microsoft.MarketplaceServicesCore.ServiceCore.Authentication.Certificate;

namespace Microsoft.MarketplaceServices.ReferenceServiceConfiguration.Service
{
    class LocalServiceConfiguration : ServiceConfigurationBase
    {
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
                    FederationXmlSource = @"Configuration\TestFederationMetadata.xml",
                    TurnoffSignatureVerificationForMetadata = true,
                };
                setting.Audiences.Add("http://audience");

                // Trusted AAD App registrations.
                var acl = new PartnerAccessControlList<string>(StringComparer.OrdinalIgnoreCase);
                acl.AddAadTrustedPartner(
                    new AadTrustedPartner(
                        applicationId: "a63e7d60-892a-44a6-95e6-0a9574b0cd15",
                        tenantId: "124edf19-b350-4797-aefc-3206115ffdb3",
                        partnerId: "RefSvc-Client-SPN"));

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
                var certAccessControl = new PartnerAccessControlList<string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "myname", "test partner" }
                };

                var aclProfile = new CommonNameAccessControlMap()
                {
                    { "Default", certAccessControl }
                };

                return aclProfile;
            }
        }

        public override Uri KeyVaultBaseUri => throw new NotImplementedException();

        public override long MaxRequestBodySize => 10485760;

        public override Guid HttpsApplicationId => new Guid("40d9d373-8927-4789-a80c-bd8b7d641f08");

        public override CertDefinition HttpsCertificate =>
            new CertDefinition
            {
                CertificateFile = "refsvctest.pfx",
                CertStoreLocation = StoreLocation.LocalMachine,
                CertStoreName = StoreName.My,
                CommonName = "refsvctest",
                Name = "Reference Service Front Door SSL Certificate",
                PasswordFile = "certificate_password.txt",
                UseSecretStore = true
            };

        public override string MdmAccountName => throw new NotImplementedException();

        public override string MdmNameSpace => throw new NotImplementedException();

        public override ThrottleConfiguration ThrottleConfiguration =>
            new ThrottleConfiguration
            {
                MaxAccepts = 10,
                MaxConnections = 10,
                RequestQueueLimit = 2000,
                RequestQueueTimeout = TimeSpan.FromSeconds(30)
            };

        public override TimeSpan VaultTimeOut => throw new NotImplementedException();

        public override string HttpPrefix => "http://localhost:8081/";

        public override string HttpsPrefix => "https://localhost:8086/";
    }
}
