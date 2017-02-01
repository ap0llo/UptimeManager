// // -----------------------------------------------------------------------------------------------------------
// //  Copyright (c) 2015-2016, Andreas Grünwald
// //  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// // -----------------------------------------------------------------------------------------------------------

using System;
using UptimeManager.Configuration.Commands;


namespace UptimeManager.Configuration
{
    class ImmutableDeviceConfiguration : IDeviceConfiguration
    {
        public string Name
        {
            get;
        }

        public CalendarType UptimeCalendarType
        {
            get;
        }

        public string CalendarProviderSettingsDirectory
        {
            get;
        }

        public string UptimeCalendarName
        {
            get;
        }

        public TimeSpan UptimeBufferInterval
        {
            get;
        }

        public ICommandSpecification IsRunningCommand
        {
            get;
        }

        public ICommandSpecification StartCommand
        {
            get;
        }

        public ICommandSpecification StopCommand
        {
            get;
        }


        public ImmutableDeviceConfiguration(string name, CalendarType uptimeCalendarType, string calendarProviderSettingsName,
            string uptimeCalendarName, TimeSpan uptimeBufferInterval, ICommandSpecification isRunningCommand, ICommandSpecification startCommand,
            ICommandSpecification stopCommand)
        {
            this.Name = name;
            this.UptimeCalendarType = uptimeCalendarType;
            this.CalendarProviderSettingsDirectory = calendarProviderSettingsName;
            this.UptimeCalendarName = uptimeCalendarName;
            this.UptimeBufferInterval = uptimeBufferInterval;
            this.IsRunningCommand = isRunningCommand;
            this.StartCommand = startCommand;
            this.StopCommand = stopCommand;
        }

    
    }
}
