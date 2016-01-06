// // -----------------------------------------------------------------------------------------------------------
// //  Copyright (c) 2015-2016, Andreas Grünwald
// //  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// // -----------------------------------------------------------------------------------------------------------

using System;
using UptimeManager.Calendar.Google;
using UptimeManager.Calendar.Model;

namespace UptimeManager.Calendar
{
    /// <summary>
    /// Factory to instantiate new instances of ICalendar (regardless of the calendar type)
    /// </summary>
    public class CalendarFactory : ICalendarFactory
    {
        /// <summary>
        /// Gets a new instance of <see cref="ICalendar" /> for the specified calendar
        /// </summary>
        public ICalendar GetCalendar(CalendarType calendarType, string calendarSettingsFilePath, string calendarName)
        {
            switch (calendarType)
            {
                case CalendarType.GoogleCalendar:
                    return new GoogleCalendar(calendarSettingsFilePath, calendarName);

                default:
                    throw new ArgumentException("Unknown CalendarType: " + calendarType);
            }
        }
    }
}