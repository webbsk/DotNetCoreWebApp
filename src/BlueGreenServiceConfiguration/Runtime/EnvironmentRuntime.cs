//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.MarketplaceServices.ReferenceServiceConfiguration.Service;
using Microsoft.MarketplaceServicesCore.Core;
using Microsoft.MarketplaceServicesCore.Core.Counters;
using Microsoft.MarketplaceServicesCore.Core.Logging;
using Microsoft.MarketplaceServicesCore.Core.Logging.Abstractions;
using Microsoft.MarketplaceServicesCore.Core.Setup;

namespace Microsoft.MarketplaceServices.ReferenceServiceConfiguration.Runtime
{
    /// <summary>
    /// Represents the configuration necessary to operate in an environment. Different environments handle critical
    /// functions like loading certificates differently. For example, containers may be created with the certificates,
    /// while in AP, certificates are loaded from the secret store.
    /// </summary>
    /// <remarks>
    /// This class should only contain members that differ from environment to environment. When something doesn't
    /// vary, it should not be present in this class, and instead should be a compile-time constant. If a property
    /// is no longer used, or no longer varies, it should be removed. If a property qualifies as a setting or
    /// configuration, use the ServiceConfiguration object.
    /// </remarks>
    public abstract class EnvironmentRuntime
    {
        /// <summary>
        /// Production environment names.
        /// </summary>
        public const string EnvProd = "PROD";

        /// <summary>
        /// Integration environment names.
        /// </summary>
        public const string EnvInt = "INT";

        /// <summary>
        /// Local environment names.
        /// </summary>
        public const string EnvLocal = "LOCAL";

        /// <summary>
        /// Gets the counter factory to use.
        /// </summary>
        public abstract ICounterFactory CounterFactory { get; }

        /// <summary>
        /// Supported environment types.
        /// </summary>
        public static IEnumerable<string> KnownEnvironmentTypes => new[] { EnvProd, EnvInt, EnvLocal };

        /// <summary>
        /// The environment type, <see cref="KnownEnvironmentTypes"/> and <see cref="EnvProd"/>.
        /// </summary>
        public string EnvironmentType { get; set; }

        /// <summary>
        /// EventLogger used to log events from application.
        /// </summary>
        public abstract IEventLogger EventLogger { get; }

        /// <summary>
        /// The environment data directory.
        /// </summary>
        public virtual string DataDirectory => Path.GetTempPath();

        /// <summary>
        /// Create a base runtime environment.
        /// </summary>
        /// <param name="environmentType">The environment type, <see cref="KnownEnvironmentTypes"/>.</param>
        public EnvironmentRuntime(string environmentType)
        {
            if (!KnownEnvironmentTypes.Any(x => x.Equals(environmentType, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentOutOfRangeException(nameof(environmentType));
            };

            this.EnvironmentType = environmentType.ToUpperInvariant();
        }

        /// <summary>
        /// Load the service configuration.
        /// </summary>
        /// <returns>The service configuration.</returns>
        public abstract ServiceConfigurationBase ServiceConfiguration { get; }

        /// <summary>
        /// Load the secret store.
        /// </summary>
        /// <returns>The secret store.</returns>
        public abstract SecretStoreBase LoadSecretStore(
            SequencedEventLogger sequencedEventLogger,
            ICounterFactory counterFactory);

        /// <summary>
        /// Logger used to log the output from setup
        /// </summary>
        public abstract ISetupLogger SetupLogger { get; }
    }
}