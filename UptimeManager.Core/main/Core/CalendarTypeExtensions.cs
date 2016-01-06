// // -----------------------------------------------------------------------------------------------------------
// //  Copyright (c) 2015-2016, Andreas Grünwald
// //  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// // -----------------------------------------------------------------------------------------------------------

using System;
using UptimeManager.Calendar;

namespace UptimeManager.Core
{
    /// <summary>
    /// Conversion methods for the differnt CalendarType enums in UptimeManager.Configuration and UptimeManager.Calendar
    /// </summary>
    static class CalendarTypeExtensions
    {
        public static CalendarType ToCalendarCalendarType(this Configuration.CalendarType configurationCalendarType)
        {
            switch (configurationCalendarType)
            {
                case Configuration.CalendarType.Unspecified:
                    return CalendarType.Unspecified;

                case Configuration.CalendarType.GoogleCalendar:
                    return CalendarType.GoogleCalendar;

                default:
                    throw new NotImplementedException("Unimplemented case in switch statement");
            }
        }

        public static Configuration.CalendarType ToConfigurationCalendartype(this CalendarType calendarCalendarType)
        {
            switch (calendarCalendarType)
            {
                case CalendarType.Unspecified:
                    return Configuration.CalendarType.Unspecified;

                case CalendarType.GoogleCalendar:
                    return Configuration.CalendarType.GoogleCalendar;

                default:
                    throw new NotImplementedException("Unimplemented case in switch statement");
            }
        }
    }
}