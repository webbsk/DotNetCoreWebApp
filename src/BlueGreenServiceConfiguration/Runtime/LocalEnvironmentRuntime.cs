//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.MarketplaceServices.ReferenceServiceConfiguration.SecretStore;
using Microsoft.MarketplaceServices.ReferenceServiceConfiguration.Service;
using Microsoft.MarketplaceServicesCore.Core;
using Microsoft.MarketplaceServicesCore.Core.Counters;
using Microsoft.MarketplaceServicesCore.Core.Logging;
using Microsoft.MarketplaceServicesCore.Core.Logging.Abstractions;
using Microsoft.MarketplaceServicesCore.Core.Setup;

namespace Microsoft.MarketplaceServices.ReferenceServiceConfiguration.Runtime
{
    /// <summary>
    /// Create a local runtime environment definition; this is to support development, debugging and testing.
    /// </summary>
    public class LocalEnvironmentRuntime : EnvironmentRuntime
    {
        readonly IEventLogger eventLogger = null;
        readonly IDictionary<string, byte[]> secrets = null;
        readonly SecretStoreBase secretstore = null;
        readonly ICounterFactory counterFactory = null;
        readonly ServiceConfigurationBase serviceConfiguration = null;
        readonly ISetupLogger setupLogger = null;

        const string CertificatePassword = "";
        readonly Dictionary<string, string> certificateBase64EncodedStrings =
            new Dictionary<string, string>()
            {
                { "refsvctest", ""
                }
            };

        /// <summary>
        /// Create a local environment definition.
        /// </summary>
        /// <param name="secrets">A collection of secrets for the environment.</param>
        public LocalEnvironmentRuntime(IDictionary<string, byte[]> secrets = null) : base(EnvLocal)
        {
            this.secrets = secrets?.ToDictionary(x => x.Key, x => x.Value) ?? new Dictionary<string, byte[]>();

            this.serviceConfiguration = new LocalServiceConfiguration();

            foreach (CertDefinition cert in this.serviceConfiguration.CertificatesToInstall)
            {
                if (this.certificateBase64EncodedStrings.TryGetValue(cert.CommonName, out string certString))
                {
                    this.secrets[cert.CertificateFile] = Convert.FromBase64String(certString);
                    this.secrets[cert.PasswordFile] = Encoding.UTF8.GetBytes(CertificatePassword);
                }
                else
                {
                    throw new InvalidOperationException("Local certificate configuration not defined in the runtime");
                }
            }

            this.eventLogger = new SllEventLogger();
            this.secretstore = new MemorySecretStore(this.secrets);
            this.counterFactory = new MemoryCounterFactory();
            this.setupLogger = new ConsoleSetupLogger();
        }

        /// <summary>
        /// CounterFactory to use.
        /// </summary>
        public override ICounterFactory CounterFactory { get => this.counterFactory; }

        /// <summary>
        /// Event logger used to log the application events.
        /// </summary>
        public override IEventLogger EventLogger { get => this.eventLogger; }

        /// <summary>
        /// Service configuration
        /// </summary>
        public override ServiceConfigurationBase ServiceConfiguration { get => this.serviceConfiguration; }

        /// <summary>
        /// Loads the secret store
        /// </summary>
        /// <param name="sequencedEventLogger">Sequenced event logger</param>
        /// <param name="counterFactory">Counter factory</param>
        public override SecretStoreBase LoadSecretStore(
            SequencedEventLogger sequencedEventLogger,
            ICounterFactory counterFactory)
                => this.secretstore;

        /// <summary>
        /// The logger used to output the log lines from setup.
        /// </summary>
        public override ISetupLogger SetupLogger { get => this.setupLogger; }
    }
}