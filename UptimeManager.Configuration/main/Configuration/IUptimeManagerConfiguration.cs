﻿// // -----------------------------------------------------------------------------------------------------------
// //  Copyright (c) 2015-2016, Andreas Grünwald
// //  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// // -----------------------------------------------------------------------------------------------------------

using System.Collections.Generic;

namespace UptimeManager.Configuration
{
    public interface IUptimeManagerConfiguration
    {
        string ConfigurationPath { get; }

        IEnumerable<IDeviceConfiguration> Devices { get; }
    }
}