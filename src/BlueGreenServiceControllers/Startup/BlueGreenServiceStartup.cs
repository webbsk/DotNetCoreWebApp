//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.MarketplaceServices.ReferenceServiceConfiguration.Runtime;
using Microsoft.MarketplaceServices.ReferenceServiceExceptions;
using Microsoft.MarketplaceServicesCore.Core;
using Microsoft.MarketplaceServicesCore.Core.Logging.Extensions;
using Microsoft.MarketplaceServicesCore.ServiceCore;
using Microsoft.MarketplaceServicesCore.ServiceCore.Authentication.Certificate;

namespace Microsoft.MarketplaceServices.ReferenceServiceControllers.Startup
{
    /// <summary>
    /// Startup class used to configure services and applications for a webhost.
    /// </summary>
    public class ReferenceServiceStartup
    {
        readonly EnvironmentRuntime environmentRuntime = null;

        /// <summary>
        /// Constructor to initialize instance of startup class.
        /// </summary>
        /// <param name="env">Hosting environment</param>
        /// <param name="environmentRuntime">Service runtime environment.</param>
        public ReferenceServiceStartup(
#pragma warning disable CA1801 // Review unused parameters
            IHostingEnvironment env,
#pragma warning restore CA1801
            EnvironmentRuntime environmentRuntime)
        {
            this.environmentRuntime = environmentRuntime;
        }

        /// <summary>
        /// Configures the service handlers, filters, and routes.
        /// </summary>
        /// <param name="app">App to configure</param>
#pragma warning disable CA1822 // Mark member as static.
        public void Configure(IApplicationBuilder app)
        {
            // Configure all the middlewares here.
            app.UseMiddleware<UnhandledExceptionMiddleware>()
               .UseInstrumentationMiddleware()
               .UseMiddleware<KeepAliveMiddleware>()
               .UseMvc();
        }
#pragma warning restore CA1822

        /// <summary>
        /// Configure dependency injection.
        /// </summary>
        /// <param name="services">Existing collection of services.</param>
        /// <returns>A service provideder.</returns>
        public void ConfigureServices(IServiceCollection services)
        {
            // Configure all the dependent services here.
            services.AddSingleton<KeepAliveConfiguration>();
            services.AddSingleton<OutOfServiceEvaluator>();

            services.AddSingleton(this.environmentRuntime.CounterFactory);
            services.AddSingleton(this.environmentRuntime.ServiceConfiguration.CommonNameAccessControlMap);

            services.AddSingleton<IAsyncClientCertificateValidator, DefaultClientCertificateValidator>()
                    .AddSingleton<IAsyncClientCertificateProvider, DefaultClientCertificateProvider>();

            services.AddTransient<ReferenceServiceExceptionProvider>()
                    .AddTransient<ServiceErrorExceptionProvider, ReferenceServiceExceptionProvider>();

            services.AddSingleton(new ReferenceServiceExceptionConverter(ReferenceServiceExceptionProvider.ErrorSource))
                    .AddSingleton<ExceptionConverter>(s => s.GetRequiredService<ReferenceServiceExceptionConverter>());

            // Inject AAD auth settings and cache so that the AadAuthenticationFilterAttribute can be used
            services.AddSingleton(this.environmentRuntime.ServiceConfiguration.AadAuthenticationSettings)
                    .AddMemoryCache();

            services.AddLogging(builder => builder.AddSequencedEventLogger(this.environmentRuntime.EventLogger));

            services.AddMvcCore(
                setupAction: (
                    options =>
                    {
                        options.Filters.Add(new DefaultMvcOperationMetadataProvider());
                        options.Filters.Add(new UnhandledExceptionActionFilterAttribute());
                        options.Filters.Add(new CommonNameCertificateAuthenticationFilterAttribute(profile: "Default"));
                    }))
                    .AddWebApiConventions()
                    .AddFormatterMappings()
                    .AddJsonFormatters();
        }
    }
}