//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.MarketplaceServices.ReferenceServiceExceptions;

namespace Microsoft.MarketplaceServices.ReferenceServiceAuthorization
{
#pragma warning disable CA1812 // Avoid uninstantiated internal classes
    class ClaimRequirementFilter : IAuthorizationFilter
#pragma warning restore CA1812 // Avoid uninstantiated internal classes
    {
        readonly string[] claimTypes;
        readonly ReferenceServiceExceptionProvider exceptionProvider;

        public ClaimRequirementFilter(string[] claimTypes, ReferenceServiceExceptionProvider exceptionProvider)
        {
            this.claimTypes = claimTypes;
            this.exceptionProvider = exceptionProvider;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            foreach (string claimType in this.claimTypes)
            {
                bool hasClaim = context.HttpContext.User.Claims.Any(c => c.Type == claimType);
                if (!hasClaim)
                {
                    throw this.exceptionProvider.ClaimMissingException(claimType);
                }
            }
        }
    }
}
