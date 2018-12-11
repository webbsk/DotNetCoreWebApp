//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using System;
using Microsoft.AspNetCore.Mvc;

namespace Microsoft.MarketplaceServices.ReferenceServiceAuthorization
{
    /// <summary>
    /// Authorization attribute to check existence of a claim
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ClaimExistsRequirementAttribute : TypeFilterAttribute
    {
        public ClaimExistsRequirementAttribute(string[] claimType) : base(typeof(ClaimRequirementFilter))
        {
            this.Arguments = new object[] { claimType };
        }
    }
}
