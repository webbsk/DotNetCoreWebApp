//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System;
using System.Linq;
using System.Net;
using Microsoft.MarketplaceServices.ReferenceServiceConfiguration.Runtime;
using Microsoft.MarketplaceServices.ReferenceServiceConfiguration.Service;
using Microsoft.MarketplaceServices.ReferenceWorker.Startup;

namespace Microsoft.MarketplaceServices.ReferenceWorker
{
    /// <summary>
    /// Initializes the ReferenceWorker service.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Main entry point.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            // Set the number of concurrent connections allowed
            // See http://msdn.microsoft.com/en-us/library/system.net.servicepointmanager.defaultconnectionlimit.aspx

            Policies.ServiceFabricContainer.Register();

            ServicePointManager.DefaultConnectionLimit = 1600;

            EnvironmentRuntime environmentRuntime = null;

            if (args.Any(x => x.Equals("-RunLocal", StringComparison.OrdinalIgnoreCase)))
            {
                // running locally
                environmentRuntime = new LocalEnvironmentRuntime();
            }
            else if (args.Any(x => x.Equals("-RunContainer", StringComparison.OrdinalIgnoreCase)))
            {
                // running in a container
                environmentRuntime = new ContainerEnvironmentRuntime();
            }
            else
            {
                throw new InvalidOperationException("Unsupported Runtime environment specified");
            }

            ServiceConfigurationBase serviceConfiguration = environmentRuntime.ServiceConfiguration;

            try
            {
                ServiceConfigurator.ConfigureAndRunWorkerService(environmentRuntime);
            }
            catch (Exception ex)
            {
                environmentRuntime.SetupLogger.LogError("Exception encountered starting the service {0}", ex);
                throw;
            }
        }
    }
}
