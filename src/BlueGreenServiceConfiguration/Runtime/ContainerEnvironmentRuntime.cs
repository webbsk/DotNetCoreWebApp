//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System;
using Microsoft.MarketplaceServices.AzureKeyVault;
using Microsoft.MarketplaceServices.AzureManagedServiceIdentity;
using Microsoft.MarketplaceServices.ReferenceServiceConfiguration.SecretStore;
using Microsoft.MarketplaceServices.ReferenceServiceConfiguration.Service;
using Microsoft.MarketplaceServicesCore.Core;
using Microsoft.MarketplaceServicesCore.Core.Counters;
using Microsoft.MarketplaceServicesCore.Core.Counters.Geneva;
using Microsoft.MarketplaceServicesCore.Core.Logging;
using Microsoft.MarketplaceServicesCore.Core.Logging.Abstractions;
using Microsoft.MarketplaceServicesCore.Core.Setup;

namespace Microsoft.MarketplaceServices.ReferenceServiceConfiguration.Runtime
{
    /// <summary>
    /// Defines a docker container runtime environment.
    /// </summary>
    public class ContainerEnvironmentRuntime : EnvironmentRuntime
    {
        readonly IEventLogger eventLogger = null;
        readonly ICounterFactory counterFactory = null;
        readonly ServiceConfigurationBase serviceConfiguration = null;
        readonly ISetupLogger setupLogger = null;
        SecretStoreBase secretStoreBase = null;

        /// <summary>
        /// Create an environment configuration for a UST container.
        /// </summary>
        public ContainerEnvironmentRuntime() : base(InferEnvironmentCluster())
        {
            this.eventLogger = new SllEventLogger();
            switch (this.EnvironmentType.ToUpperInvariant())
            {
                case EnvInt:
                    this.serviceConfiguration = new IntegrationServiceConfiguration();
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported EnvironmentType {this.EnvironmentType}");
            }

            this.counterFactory = new GenevaCounterFactory(
                this.serviceConfiguration.MdmAccountName,
                this.serviceConfiguration.MdmNameSpace);

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
        /// Container environment types (prod, int, etc) are inferred from the Environment variable.
        /// </summary>
        /// <returns>The autopilot cluster name, eg: int.</returns>
        static string InferEnvironmentCluster()
            => Environment.GetEnvironmentVariable("Environment")?.ToUpperInvariant();

        /// <summary>
        /// Service configuration to use.
        /// </summary>
        public override ServiceConfigurationBase ServiceConfiguration { get => this.serviceConfiguration; }

        public override SecretStoreBase LoadSecretStore(
            SequencedEventLogger sequencedEventLogger,
            ICounterFactory counterFactory)
        {
            if (this.secretStoreBase == null)
            {
                if (this.serviceConfiguration.KeyVaultBaseUri == null)
                {
                    throw new ArgumentException(nameof(this.serviceConfiguration.KeyVaultBaseUri));
                }

                var msiSettings = new AzureManagedServiceIdentitySettings
                {
                    BaseAddress = this.serviceConfiguration.MsiBaseUri,
                    CallingServiceName = "ReferenceFD",
                    Timeout = this.serviceConfiguration.VaultTimeOut
                };

                var keyVaultSettings = new AzureKeyVaultClientSettings
                {
                    AuthenticationType = AzureActiveDirectoryAuthenticationType.ManagedServiceIdentity,
                    Timeout = this.serviceConfiguration.VaultTimeOut,
                    VaultBaseUri = this.serviceConfiguration.KeyVaultBaseUri,
                };

                this.secretStoreBase = new AzureKeyVaultStore(
                    new AzureKeyVaultClient(keyVaultSettings,
                        new AzureManagedServiceIdentityClient(
                            msiSettings,
                            sequencedEventLogger,
                            counterFactory),
                        sequencedEventLogger,
                        counterFactory));
            }

            return this.secretStoreBase;
        }

        /// <summary>
        /// The logger used to output the log lines from setup.
        /// </summary>
        public override ISetupLogger SetupLogger { get => this.setupLogger; }
    }
}