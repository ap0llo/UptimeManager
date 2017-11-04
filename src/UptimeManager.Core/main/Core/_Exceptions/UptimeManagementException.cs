// -----------------------------------------------------------------------------------------------------------
//  Copyright (c) 2015-2017, Andreas Grünwald
//  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// -----------------------------------------------------------------------------------------------------------

using System;

namespace UptimeManager.Core
{
    public class UptimeManagementException : Exception
    {
        public UptimeManagementException(string message)
            : base(message)
        {
        }

        public UptimeManagementException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}