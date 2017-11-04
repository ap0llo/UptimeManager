// -----------------------------------------------------------------------------------------------------------
//  Copyright (c) 2015-2017, Andreas Grünwald
//  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// -----------------------------------------------------------------------------------------------------------

using System;
using UptimeManager.Configuration.Commands;

namespace UptimeManager.Configuration
{
    /// <summary>
    /// Configuration entry for a device
    /// </summary>
    public interface IDeviceConfiguration
    {
        /// <summary>
        /// The name of the device
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The type of calendar used to configure uptime for the device
        /// </summary>
        CalendarType UptimeCalendarType { get; }

        /// <summary>
        /// The path of the directory that can be used by the calendar implentation to store configuration
        /// </summary>
        string CalendarProviderSettingsDirectory { get; }

        /// <summary>
        /// The name of the calendar used to configure uptime for the device
        /// </summary>
        string UptimeCalendarName { get; }

        /// <summary>
        /// The interval to wait after the last uptime event is no longer actice before shutting down the device
        /// and the interval to start the device before a uptime event becomes active
        /// </summary>
        TimeSpan UptimeBufferInterval { get; }

        /// <summary>
        /// The command to execute to check if the device is currently runnning
        /// </summary>
        ICommandSpecification IsRunningCommand { get; }

        /// <summary>
        /// The command that starts the device
        /// </summary>
        ICommandSpecification StartCommand { get; }

        /// <summary>
        /// The command that shuts down the device
        /// </summary>
        ICommandSpecification StopCommand { get; }
    }
}