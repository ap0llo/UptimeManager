// // -----------------------------------------------------------------------------------------------------------
// //  Copyright (c) 2015-2016, Andreas Grünwald
// //  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// // -----------------------------------------------------------------------------------------------------------

using UptimeManager.Calendar.Model;
using UptimeManager.Configuration;
using UptimeManager.Core;

namespace UptimeManager.Calendar
{
    public static class CalendarFactoryExtensions
    {
        public static ICalendar GetCalendar(this ICalendarFactory factory, Configuration.CalendarType calendarType, 
                                            string calendarSettingsFilePath, string calendarName)
        {
            return factory.GetCalendar(calendarType.ToCalendarCalendarType(), calendarSettingsFilePath, calendarName);
        }

        public static ICalendar GetCalendar(this ICalendarFactory factory, IDeviceConfiguration device)
        {
            return factory.GetCalendar(device.UptimeCalendarType, device.CalendarProviderSettingsDirectory, device.UptimeCalendarName);
        }
    }
}