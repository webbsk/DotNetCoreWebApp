//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using Microsoft.MarketplaceServices.ReferenceServiceConfiguration.Runtime;
using Microsoft.MarketplaceServices.ReferenceServiceConfiguration.SecretStore;
using Microsoft.MarketplaceServices.ReferenceServiceConfiguration.Service;
using Microsoft.MarketplaceServicesCore.Core;
using Microsoft.MarketplaceServicesCore.Core.Counters;
using Microsoft.MarketplaceServicesCore.Core.Logging;
using Microsoft.MarketplaceServicesCore.Core.Logging.Abstractions;
using Microsoft.MarketplaceServicesCore.Core.Setup;

namespace Microsoft.MarketplaceServices.ReferenceServiceTestsCommon.Configuration
{
    /// <summary>
    /// Defines a test environment runtime definition.
    /// </summary>
    public class TestEnvironmentRuntime : EnvironmentRuntime
    {
        readonly IEventLogger eventLogger = null;
        readonly SecretStoreBase secretstore = null;
        readonly ICounterFactory counterFactory = null;
        readonly ServiceConfigurationBase serviceConfiguration = null;
        readonly ISetupLogger setupLogger = null;

        /// <summary>
        /// Create an empty test environment definition.
        /// </summary>
        public TestEnvironmentRuntime() : base(EnvLocal)
        {
            this.counterFactory = new MemoryCounterFactory();
            this.eventLogger = new SllEventLogger();
            this.serviceConfiguration = new TestServiceConfiguration();
            this.secretstore = new MemorySecretStore();
            this.setupLogger = new ConsoleSetupLogger();
        }

        /// <summary>
        /// Coutner factory to use.
        /// </summary>
        public override ICounterFactory CounterFactory
        {
            get
            {
                return this.counterFactory;
            }
        }

        /// <summary>
        /// Event logger used to log the application events.
        /// </summary>
        public override IEventLogger EventLogger
        {
            get
            {
                return this.eventLogger;
            }
        }

        /// <summary>
        /// Service configuration to use.
        /// </summary>
        public override ServiceConfigurationBase ServiceConfiguration
        {
            get
            {
                return this.serviceConfiguration;
            }
        }

        /// <summary>
        /// Secret store
        /// </summary>
        /// <param name="sequencedEventLogger">Sequenced event logger</param>
        /// <param name="counterFactory">Counterfactory to use</param>
        /// <returns></returns>
        public override SecretStoreBase LoadSecretStore(
            SequencedEventLogger sequencedEventLogger, ICounterFactory counterFactory)
        {
            return this.secretstore;
        }

        /// <summary>
        /// The logger used to output the log lines from setup.
        /// </summary>
        public override ISetupLogger SetupLogger
        {
            get
            {
                return this.setupLogger;
            }
        }
    }
}