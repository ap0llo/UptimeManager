// -----------------------------------------------------------------------------------------------------------
//  Copyright (c) 2015-2017, Andreas Grünwald
//  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// -----------------------------------------------------------------------------------------------------------

namespace UptimeManager.Configuration
{
    /// <summary>
    /// Interface for reading configuration files
    /// </summary>
    public interface IConfigurationReader
    {
        /// <summary>
        /// Reads configuration from the specified file
        /// </summary>
        IUptimeManagerConfiguration ReadConfiguration(string filePath);
    }
}