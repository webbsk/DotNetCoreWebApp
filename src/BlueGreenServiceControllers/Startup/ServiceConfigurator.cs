//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.MarketplaceServices.ReferenceServiceConfiguration.Runtime;
using Microsoft.MarketplaceServices.ReferenceServiceConfiguration.Service;
using Microsoft.MarketplaceServicesCore.ServiceCore;

namespace Microsoft.MarketplaceServices.ReferenceServiceControllers.Startup
{
    /// <summary>
    /// Wrapper class to configure the service parameters.
    /// </summary>
    public static class ServiceConfigurator
    {
        /// <summary>
        /// Configures ans starts the http listener based on service settings.
        /// </summary>
        /// <param name="environmentRuntime">Environment runtime.</param>
        public static IWebHost ConfigureAndStartListener(EnvironmentRuntime environmentRuntime, WaitHandleSignal signal)
        {
            if (environmentRuntime == null)
            {
                throw new ArgumentNullException(nameof(environmentRuntime));
            }

            IWebHost host = BuildWebHost(environmentRuntime, signal);
            host.Start();

            return host;
        }

        /// <summary>
        /// Configures and runs the service based on service settings.
        /// </summary>
        /// <param name="environmentRuntime">Service specific settings.</param>
        public static void ConfigureAndRunService(
            EnvironmentRuntime environmentRuntime)
        {
            if (environmentRuntime == null)
            {
                throw new ArgumentNullException(nameof(environmentRuntime));
            }

            // run the service host
            using (var signal = new WaitHandleSignal())
            using (IWebHost host = BuildWebHost(environmentRuntime, signal))
            {
                // show ip information, useful for debugging containers
                environmentRuntime.SetupLogger.LogInfo($"Hostname: {Dns.GetHostName()}");
                foreach (IPAddress a in Dns.GetHostAddresses(Dns.GetHostName()))
                {
                    environmentRuntime.SetupLogger.LogInfo($"IP: {a.ToString()}");
                }

                host.Run();
            }
        }

        static IWebHost BuildWebHost(EnvironmentRuntime environmentRuntime, WaitHandleSignal signal)
        {
            var monitor = ControlBreakMonitor.Start(signal);
            ServiceConfigurationBase serviceConfiguration
                = environmentRuntime.ServiceConfiguration;

            IWebHost host = WebHost.CreateDefaultBuilder()
                 .ConfigureServices(
                     s =>
                     {
                         s.AddSingleton<EnvironmentRuntime>(environmentRuntime);
                         s.AddSingleton<ControlBreakMonitor>(monitor);
                     })
                 .UseHttpSys(
                     options =>
                     {
                         options.Authentication.Schemes = AspNetCore.Server.HttpSys.AuthenticationSchemes.None;
                         options.Authentication.AllowAnonymous = true;
                         options.MaxConnections = null;
                         options.MaxRequestBodySize = serviceConfiguration.MaxRequestBodySize;

                         if (serviceConfiguration != null)
                         {
                             // throttle settings
                             options.MaxAccepts = serviceConfiguration.ThrottleConfiguration.MaxAccepts;
                             options.MaxConnections =
                                 serviceConfiguration.ThrottleConfiguration.MaxConnections;
                             options.RequestQueueLimit =
                                 serviceConfiguration.ThrottleConfiguration.RequestQueueLimit;
                             options.Timeouts.RequestQueue =
                                 serviceConfiguration.ThrottleConfiguration.RequestQueueTimeout;
                         }

                         options.Http503Verbosity = Http503VerbosityLevel.Limited;

                         // Will suppress 5xx when the clinet has disconnected already
                         options.ThrowWriteExceptions = false;
                         options.EnableResponseCaching = false;
                     })
                     .UseUrls(serviceConfiguration.AllPrefixes.ToArray())
                 .UseStartup<ReferenceServiceStartup>()
                 .Build();

            return host;
        }
    }
}