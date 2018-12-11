//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.MarketplaceServices.ReferenceServiceConfiguration.Runtime;
using Microsoft.MarketplaceServices.ReferenceServiceConfiguration.Service;
using Microsoft.MarketplaceServices.ReferenceWorker.WorkerServices;
using Microsoft.MarketplaceServicesCore.ServiceCore;

namespace Microsoft.MarketplaceServices.ReferenceWorker.Startup
{
    /// <summary>
    /// Wrapper class to configure the service parameters.
    /// </summary>
    public static class ServiceConfigurator
    {
        /// <summary>
        /// Configures and runs the worker service based on service settings.
        /// </summary>
        /// <param name="environmentRuntime">Service specific settings.</param>
        public static void ConfigureAndRunWorkerService(
            EnvironmentRuntime environmentRuntime)
        {
            if (environmentRuntime == null)
            {
                throw new ArgumentNullException(nameof(environmentRuntime));
            }

            // run the worker service
            using(var signal = new WaitHandleSignal())
            using (IHost host = BuildWorkerHost(environmentRuntime, signal))
            {
                host.Run();
            }
        }

        static IHost BuildWorkerHost(EnvironmentRuntime environmentRuntime, WaitHandleSignal signal)
        {
            var monitor = ControlBreakMonitor.Start(signal);
            ServiceConfigurationBase serviceConfiguration = environmentRuntime.ServiceConfiguration;

            IHost host = new HostBuilder()
                .ConfigureServices(
                    s =>
                    {
                        s.AddSingleton<EnvironmentRuntime>(environmentRuntime);
                        s.AddSingleton<ControlBreakMonitor>(monitor);
                        s.AddHostedService<HeartbeatWorkerService>();
                    })
                .Build();

            return host;
        }
    }
}