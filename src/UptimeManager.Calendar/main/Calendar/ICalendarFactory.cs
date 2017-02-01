// // -----------------------------------------------------------------------------------------------------------
// //  Copyright (c) 2015-2016, Andreas Grünwald
// //  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// // -----------------------------------------------------------------------------------------------------------

using UptimeManager.Calendar.Model;

namespace UptimeManager.Calendar
{
    public interface ICalendarFactory
    {
        /// <summary>
        /// Gets a new instance of <see cref="ICalendar" /> for the specified calendar
        /// </summary>
        ICalendar GetCalendar(CalendarType calendarType, string calendarSettingsFilePath, string calendarName);
    }
}