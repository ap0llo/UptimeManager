// -----------------------------------------------------------------------------------------------------------
//  Copyright (c) 2015-2017, Andreas Grünwald
//  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// -----------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace UptimeManager.Configuration
{
    public interface IUptimeManagerConfiguration
    {
        string FilePath { get; }

        IEnumerable<IDeviceConfiguration> Devices { get; }
    }
}