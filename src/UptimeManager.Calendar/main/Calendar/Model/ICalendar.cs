// -----------------------------------------------------------------------------------------------------------
//  Copyright (c) 2015-2017, Andreas Grünwald
//  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// -----------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

namespace UptimeManager.Calendar.Model
{
    public interface ICalendar
    {
        /// <summary>
        /// The name of the calendar
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Connectes to the calendar's backend service (for calendar services using OAuth, the user might need to enter
        /// information using a web browser)
        /// </summary>
        void ConnectToService();

        /// <summary>
        /// Gets the events from the calendar, beginnig at the specified start date.
        /// For recurring events, only the next occurrence is included in the result
        /// </summary>
        IEnumerable<ICalendarEvent> GetEvents(DateTime startDate);

        /// <summary>
        /// Gets the event with the specified id
        /// </summary>
        ICalendarEvent GetEvent(string eventId);

        /// <summary>
        /// Adds a new event to the calendar
        /// </summary>
        ICalendarEvent AddEvent(string eventName, DateTime start, DateTime end);

        /// <summary>
        /// Updates the specified event
        /// </summary>
        /// <remarks>Recurring events are not supported.</remarks>
        ICalendarEvent UpdateEvent(ICalendarEvent eventToUpdate);
    }
}