// -----------------------------------------------------------------------------------------------------------
//  Copyright (c) 2015-2017, Andreas Grünwald
//  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// -----------------------------------------------------------------------------------------------------------

using System;

namespace UptimeManager.Configuration
{
    /// <summary>
    /// Represents an error that occurred while processing configuration
    /// </summary>
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message)
            : base(message)
        {
        }

        public ConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}