//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.MarketplaceServices.ReferenceServiceConfiguration.Runtime;
using Microsoft.MarketplaceServices.ReferenceServiceConfiguration.Service;
using Microsoft.MarketplaceServicesCore.Core;
using Microsoft.MarketplaceServicesCore.Core.Counters;
using Microsoft.MarketplaceServicesCore.Core.Logging;
using Microsoft.MarketplaceServicesCore.Core.Setup;

namespace Microsoft.MarketplaceServices.ReferenceServiceSetup
{
    /// <summary>
    /// Starting point for setup.
    /// </summary>
    public static class Program
    {
        public static int Main(string[] args)
        {
            // don't know runtime yet, so default to local
            EnvironmentRuntime environmentRuntime;

            if (args.Any(x => x.Equals("-RunLocal", StringComparison.OrdinalIgnoreCase)))
            {
                // running locally
                environmentRuntime = new LocalEnvironmentRuntime();
            }
            else if (args.Any(x => x.Equals("-RunContainer", StringComparison.OrdinalIgnoreCase)))
            {
                // running in autopilot
                environmentRuntime = new ContainerEnvironmentRuntime();
            }
            else
            {
                throw new InvalidOperationException("Unsupported Runtime environment specified");
            }

            ServiceConfigurationBase serviceConfiguration = environmentRuntime.ServiceConfiguration;
            var logger = new SequencedEventLogger(new SllEventLogger());
            ICounterFactory counterFactory = environmentRuntime.CounterFactory;
            SecretStoreBase secretStore = environmentRuntime.LoadSecretStore(logger, counterFactory);

            ExitCode exitCode = ExitCode.UnspecifiedError;
            try
            {
                exitCode = ExitCode.SslCertInstallationFailed;

                foreach (CertDefinition certificate in serviceConfiguration.CertificatesToInstall)
                {
                    CertificateSetup.InstallCertificate(
                        certificate, secretStore, environmentRuntime.SetupLogger);
                }

                if (serviceConfiguration.HttpsCertificate != null)
                {
                    string sslPort =
                        new Regex(@":(\d+)").Match(serviceConfiguration.HttpsPrefix)?.Value.Substring(1) ?? "443";

                    if (!CertificateSetup.TryBindSslCert(
                        serviceConfiguration.HttpsCertificate,
                        serviceConfiguration.HttpsApplicationId,
                        sslPort,
                        secretStore,
                        environmentRuntime.SetupLogger))
                    {
                        exitCode = ExitCode.SslCertInstallationFailed;
                    }
                }

                exitCode = ExitCode.Ok;
            }
            catch (Exception e)
            {
                environmentRuntime.SetupLogger.LogError("An unhandled exception occurred: {0}", e);
            }

            return (int)exitCode;
        }
    }
}