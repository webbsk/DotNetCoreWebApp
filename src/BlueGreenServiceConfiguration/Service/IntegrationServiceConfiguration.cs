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
    class IntegrationServiceConfiguration : ServiceConfigurationBase
    {
        public override AadAuthenticationSettings AadAuthenticationSettings
        {
            get
            {
                var settings = new AadAuthenticationSettings
                {
                    FederationMetadataCacheSettings = new FederationMetadataCacheSettings
                    {
                        CacheTime = TimeSpan.FromDays(1),
                        FederationMetadataClient = new FederationMetadataClient()
                    },
                    FederationXmlSource =
                       "https://login.windows.net/common/federationmetadata/2007-06/federationmetadata.xml",
                    TurnoffSignatureVerification = false
                };
                settings.Audiences.Add("https://onestore-df.microsoft.com");

                // Trusted AAD App registrations.
                var acl = new PartnerAccessControlList<string>(StringComparer.OrdinalIgnoreCase);
                acl.AddAadTrustedPartner(
                    new AadTrustedPartner(
                        applicationId: "a63e7d60-892a-44a6-95e6-0a9574b0cd15",
                        tenantId: "124edf19-b350-4797-aefc-3206115ffdb3",
                        partnerId: "RefSvc-Client-SPN"));

                settings.TrustedPartnerMap.TryAddProfile(
                    profile: "Default",
                    accessControlList: acl);

                return settings;
            }
        }

        public override CommonNameAccessControlMap CommonNameAccessControlMap
        {
            get
            {
                // Add cert common name and a corresponding friendly name.
                var certAccessControl = new PartnerAccessControlList<string>(StringComparer.OrdinalIgnoreCase)
                {
                    { "refsvc-client-mp-microsoft-com", "Reference service test client" }
                };

                // Add a profile. Profile is a string which you can use to identify ACL.
                // For ex: you could have read, write and default profile.
                var aclProfile = new CommonNameAccessControlMap()
                {
                    { "Default", certAccessControl }
                };

                return aclProfile;
            }
        }

        public override Uri KeyVaultBaseUri => new Uri("https://mp-refsvc-int-wusf-kv.vault.azure.net/");

        public override long MaxRequestBodySize => 10485760;

        public override Guid HttpsApplicationId => new Guid("40d9d373-8927-4789-a80c-bd8b7d641f08");

        public override CertDefinition HttpsCertificate =>
            new CertDefinition
            {
                CertificateFile = "refsvc-primary",
                CertStoreLocation = StoreLocation.LocalMachine,
                CertStoreName = StoreName.My,
                CommonName = "refsvc.primary.local",
                Name = "Reference Service Front Door SSL Certificate",
                UseSecretStore = true
            };

        public override string MdmAccountName => "USTReferenceService";

        public override string MdmNameSpace => "RefSvc/Counters";

        public override ThrottleConfiguration ThrottleConfiguration =>
            new ThrottleConfiguration
            {
                MaxAccepts = 10,
                MaxConnections = 10,
                RequestQueueLimit = 2000,
                RequestQueueTimeout = TimeSpan.FromSeconds(30)
            };

        public override TimeSpan VaultTimeOut => TimeSpan.FromSeconds(5);

        public override string HttpPrefix => "http://+:80/";

        public override string HttpsPrefix => "https://+:443/";
    }
}
