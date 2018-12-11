//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.MarketplaceServices.ReferenceServiceClient;
using Microsoft.MarketplaceServices.ReferenceServiceClient.V1;
using Microsoft.MarketplaceServices.ReferenceServiceClient.V2;
using Microsoft.MarketplaceServices.ReferenceServiceConfiguration.Runtime;
using Microsoft.MarketplaceServices.ReferenceServiceConfiguration.Service;
using Microsoft.MarketplaceServices.ReferenceServiceControllers.Startup;
using Microsoft.MarketplaceServices.ReferenceServiceTestsCommon.Configuration;
using Microsoft.MarketplaceServicesCore.Core;
using Microsoft.MarketplaceServicesCore.Core.Counters;
using Microsoft.MarketplaceServicesCore.Core.Logging;
using Microsoft.MarketplaceServicesCore.Core.Logging.Extensions;
using Microsoft.MarketplaceServicesCore.Core.Setup;
using Microsoft.MarketplaceServicesCore.ServiceCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.MarketplaceServices.ReferenceServiceTests
{
    /// <summary>
    /// Assembly Init class for tests.
    /// </summary>
    [TestClass]
    public static class AssemblyInit
    {
        static IWebHost host;
        static WaitHandleSignal waitHandleSignal;

        /// <summary>
        /// Calling service name
        /// </summary>
        public const string CallingServiceName = "ReferenceServiceTests";

        /// <summary>
        /// Test environment runtime to use.
        /// </summary>
        public static EnvironmentRuntime EnvironmentRuntime { get; private set; }

        /// <summary>
        /// Memory counter factory.
        /// </summary>
        public static MemoryCounterFactory CounterFactory { get; set; }

        /// <summary>
        /// Reference service settings.
        /// </summary>
        public static ReferenceClientSettings ReferenceClientSettings { get; private set; }

        /// <summary>
        /// V1 Reference service client.
        /// </summary>
        public static ReferenceClientV1 ReferenceClientV1 { get; private set; }

        /// <summary>
        /// V2 Reference service client.
        /// </summary>
        public static ReferenceClientV2 ReferenceClientV2 { get; private set; }

        /// <summary>
        /// Sequenced Event logger
        /// </summary>
        static SequencedEventLogger EventLogger { get; set; }

        /// <summary>
        /// Logger Factory
        /// </summary>
        public static ILoggerFactory GetLoggerFactory(LoggingContext loggingContext = null)
        {
            var httpContextAccessor = new HttpContextAccessor
            {
                HttpContext = new DefaultHttpContext()
            };
            httpContextAccessor.HttpContext.SetLoggingContext(loggingContext);

            return new ServiceCollection()
                .AddSingleton<IHttpContextAccessor>(httpContextAccessor)
                .AddLogging(builder => builder.AddSequencedEventLogger(EventLogger).SetMinimumLevel(LogLevel.Trace))
                .BuildServiceProvider()
                .GetService<ILoggerFactory>();
        }

        /// <summary>
        /// Test Cleanup
        /// </summary>
        [AssemblyCleanup]
        public static void AssemblyCleanup()
        {
            TestLoggingEnvironment.Cleanup();
            if (waitHandleSignal != null)
            {
                waitHandleSignal.Dispose();
            }

            if (host != null)
            {
                host.Dispose();
            }

            ResetSslCert(EnvironmentRuntime.ServiceConfiguration.HttpsCertificate);
            RemoveSelfSignedCertFromRoot(EnvironmentRuntime.ServiceConfiguration.HttpsCertificate);
        }

        /// <summary>
        /// Reset the test state
        /// </summary>
        public static void Reset()
        {
            ResetCounters();
            TestLoggingEnvironment.Reset();
        }

        /// <summary>
        /// Reset the counter factory.
        /// </summary>
        public static void ResetCounters()
        {
            if (AssemblyInit.CounterFactory is MemoryCounterFactory memoryCounterFactory)
            {
                memoryCounterFactory.ResetCounters();
            }
        }

        /// <summary>
        /// Initializes the singletons used in test and other test settings
        ///
        /// </summary>
        /// <param name="context">Test context</param>
        [AssemblyInitialize]
        public static void AssemblyInitialize(
#pragma warning disable CA1801 // Review unused parameters
            TestContext context)
#pragma warning restore CA1801
        {
            TestLoggingEnvironment.Initialize();
            CounterFactory = new MemoryCounterFactory();
            EnvironmentRuntime = new TestEnvironmentRuntime();
            EventLogger = new SequencedEventLogger(new SllEventLogger());
            waitHandleSignal = new WaitHandleSignal();

            ServiceConfigurationBase configuration = EnvironmentRuntime.ServiceConfiguration;

            ReferenceClientSettings = new ReferenceClientSettings
            {
                BaseAddress = new Uri(configuration.HttpsPrefix),
                CallingServiceName = CallingServiceName,
                IgnoreSSLErrors = true,
                Timeout = Debugger.IsAttached ? TimeSpan.FromDays(7) : TimeSpan.FromSeconds(30),
            };

            var clientHandler = new ReferenceClientHandler(
                AssemblyInit.ReferenceClientSettings,
                GetLoggerFactory().CreateLogger<ReferenceClientHandler>(),
                AssemblyInit.CounterFactory,
                certificateLoader: new X509StoreCertificateLoader(
                    EnvironmentRuntime.ServiceConfiguration.HttpsCertificate));

            ReferenceClientV1 = new ReferenceClientV1(ReferenceClientSettings, clientHandler);
            ReferenceClientV2 = new ReferenceClientV2(ReferenceClientSettings, clientHandler);

            SetupSelfSignedTestCertAsRoot(configuration.HttpsCertificate);

            SetupSslCert(configuration.HttpsCertificate, configuration.HttpsApplicationId, port: "8086");

            host = ServiceConfigurator.ConfigureAndStartListener(EnvironmentRuntime, waitHandleSignal);
        }

        static void SetupSslCert(CertDefinition cert, Guid sslApplicationId, string port)
        {
            // Install the test certificate used in the test
            InstallCertificate(cert);

            X509Certificate2 certificate = GetCertificate(cert);

            // We will try to install the certificate for SLL Binding
            // unfortunately this required admin priviledge, but this step is only needed once per machine
            if (certificate == null)
            {
                InstallCertificate(cert);
                certificate = GetCertificate(cert);
            }

            // found/installed the certificate in LocalSystem
            // Now check if it is binded properly to the port
            if (certificate != null)
            {
                if (!NetshConfig.TryBindSslCertWithCustomPort(
                    certificate,
                    cert.Name,
                    sslApplicationId,
                    port,
                    EnvironmentRuntime.SetupLogger))
                {
                    if (!IsAdmin())
                    {
                        throw new InvalidOperationException(
                            "You must run in admin/elevated mode to setup SSL Cert binding. " +
                            "Please relaunch visual studio in elevated mode.");
                    }
                }
            }
            else
            {
                if (!IsAdmin())
                {
                    throw new InvalidOperationException(
                        "You must run in admin/elevated mode to setup Certificate. " +
                        "This is required only once per machine. " +
                        "Please relaunch visual studio in elevated mode.");
                }
            }
        }

        static void ResetSslCert(CertDefinition cert)
        {
            RemoveCertificate(cert);
            ServicePointManager.ServerCertificateValidationCallback = null;
        }

        static X509Certificate2 GetCertificate(CertDefinition certDefinition)
        {
            var certStore = new X509Store(certDefinition.CertStoreName, certDefinition.CertStoreLocation);
            certStore.Open(OpenFlags.ReadOnly);

            X509Certificate2Collection certificates = certStore.Certificates.Find(
                X509FindType.FindBySubjectName,
                certDefinition.CommonName,
                validOnly: false);

            return certificates.Count == 0 ? null : certificates[0];
        }

        static void SetupSelfSignedTestCertAsRoot(CertDefinition certDefinition)
        {
            InstallCertificate(certDefinition, StoreLocation.LocalMachine, StoreName.Root);
        }

        static void InstallCertificate(CertDefinition certDefinition)
        {
            InstallCertificate(certDefinition, certDefinition.CertStoreLocation, certDefinition.CertStoreName);
        }

        static void InstallCertificate(
            CertDefinition certDefinition,
            StoreLocation location,
            StoreName storeName)
        {
            string fileName = Path.Combine(certDefinition.SecureShare, certDefinition.CertificateFile);

            X509KeyStorageFlags flags = X509KeyStorageFlags.PersistKeySet;
            if (location == StoreLocation.LocalMachine)
            {
                flags |= X509KeyStorageFlags.MachineKeySet;
            }

            var cert = new X509Certificate2(fileName, String.Empty, flags);
            var store = new X509Store(storeName, location);
            store.Open(OpenFlags.ReadWrite);
            store.Add(cert);
            store.Close();
        }

        static bool IsAdmin()
        {
            var currentIdentity = WindowsIdentity.GetCurrent();
            return currentIdentity != null &&
                   new WindowsPrincipal(currentIdentity).IsInRole(WindowsBuiltInRole.Administrator);
        }

        static void RemoveSelfSignedCertFromRoot(CertDefinition certDefinition)
        {
            RemoveCertificate(certDefinition, StoreLocation.LocalMachine, StoreName.Root);
        }

        static void RemoveCertificate(CertDefinition certDefinition)
        {
            RemoveCertificate(certDefinition, certDefinition.CertStoreLocation, certDefinition.CertStoreName);
        }

        static void RemoveCertificate(CertDefinition certDefinition, StoreLocation location, StoreName storeName)
        {
            var fileName = Path.Combine(certDefinition.SecureShare, certDefinition.CertificateFile);

            var certCollection = new X509Certificate2Collection();
            certCollection.Import(fileName, String.Empty, X509KeyStorageFlags.PersistKeySet);
            X509Certificate2 cert = certCollection.Find(
                X509FindType.FindBySubjectName, certDefinition.CommonName, validOnly: false)[0];

            RemoveCertificate(cert, location, storeName);
        }

        static void RemoveCertificate(X509Certificate2 certificate, StoreLocation location, StoreName storeName)
        {
            var store = new X509Store(storeName, location);
            store.Open(OpenFlags.ReadWrite);
            if (store.Certificates.Contains(certificate))
            {
                store.Remove(certificate);
            }

            store.Close();
        }
    }
}
