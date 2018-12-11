//----------------------------------------------------------------
// <copyright company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//----------------------------------------------------------------

using Microsoft.MarketplaceServicesCore.ServiceCore;

namespace Microsoft.MarketplaceServices.ReferenceServiceExceptions
{
    /// <summary>
    /// Reference service exception converter. Any custom exception converter logic can go in here.
    /// </summary>
    public class ReferenceServiceExceptionConverter : ExceptionConverter
    {
        public ReferenceServiceExceptionConverter(string faultSource) : base(faultSource)
        {
        }
    }
}
