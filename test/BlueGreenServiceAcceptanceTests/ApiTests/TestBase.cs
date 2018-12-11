//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.MarketplaceServicesCore.Core.Logging;

namespace Microsoft.MarketplaceServices.ReferenceServiceTests
{
    /// <summary>
    /// Defines a base integration environment test.
    /// </summary>
    public abstract class TestBase
    {
        static readonly SigningCredentials TestAadSigningCredentials = new SigningCredentials(
            new X509SecurityKey(new X509Certificate2(@"AadCert\TestAad.pfx")),
            "http://www.w3.org/2001/04/xmldsig-more#rsa-sha256");

        /// <summary>
        /// Logging context used within tests.
        /// </summary>
        /// <remarks>Initialize it in TestInit</remarks>
        protected LoggingContext LoggingContext
        {
            get;
            private set;
        }

        /// <summary>
        /// LoggerFactory used within tests.
        /// </summary>
        /// <remarks>Initialize it in TestInit</remarks>
        protected ILoggerFactory LoggerFactory
        {
            get;
            private set;
        }

        /// <summary>
        /// Random class used to generate input identifiers to test.
        /// </summary>
        protected static Random Random
        {
            get
            {
                return new Random();
            }
        }

        /// <summary>
        /// Creates a signed Jwt Token
        /// </summary>
        protected static JwtSecurityToken CreateSignedToken(
            string issuer,
            string audience,
            List<Claim> claims = null)
        {
            return new JwtSecurityToken(
                issuer,
                audience,
                claims,
                DateTime.UtcNow,
                DateTime.UtcNow.AddMinutes(5),
                TestAadSigningCredentials);
        }

        /// <summary>
        /// Base cleanup to be invoked from derived tests
        /// </summary>
        protected void DoCleanup()
        {
            AssemblyInit.Reset();
            this.LoggingContext = null;
        }

        /// <summary>
        /// BAse initialize to be invoked from derived tests.
        /// </summary>
        /// <param name="RequestPath">Request Url</param>
        protected void DoInitialize(string RequestPath = null)
        {
            this.LoggingContext = new LoggingContext(
                correlationId: Guid.NewGuid(),
                correlationVector: null,
                requestId: Guid.NewGuid(),
                requestUri: RequestPath,
                requestMethod: null,
                contractVersion: null,
                scenarioHeader: null,
                users: Random.Next().ToString("x", CultureInfo.InvariantCulture));

            this.LoggerFactory = AssemblyInit.GetLoggerFactory(this.LoggingContext);
        }
    }
}