// -----------------------------------------------------------------------------------------------------------
//  Copyright (c) 2015-2017, Andreas Grünwald
//  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// -----------------------------------------------------------------------------------------------------------

using System;
using Google.Apis.Calendar.v3.Data;
using UptimeManager.Calendar.Model;

namespace UptimeManager.Calendar.Google
{
    /// <summary>
    /// Extension methods for types from the Google Calendar client library
    /// </summary>
    static class Extensions
    {
        /// <summary>
        /// Creates a new <see cref="ICalendarEvent" /> object from the specified Goolge Calendar event
        /// </summary>
        /// <param name="gDataEvent">The event to create a event object from</param>
        /// <param name="fromSeries">Specifies whether the event is part of a series</param>
        public static ICalendarEvent ToCalendarEvent(this Event gDataEvent, bool fromSeries = false)
            => new CalendarEvent(gDataEvent.Id, gDataEvent.Creator.ToCalendarUser(), fromSeries)
            {
                Name = gDataEvent.Summary,
                Start = gDataEvent.Start.ToDateTime(),
                End = gDataEvent.End.ToDateTime()
            };


        /// <summary>
        /// Creates a new <see cref="ICalendarUser" /> object from the specified Google <see cref="Event.CreatorData" /> object
        /// </summary>
        public static ICalendarUser ToCalendarUser(this Event.CreatorData gDataCreator)
            => new ImmutableCalendarUser(gDataCreator.DisplayName, gDataCreator.Email);



        /// <summary>
        /// Creates a DateTime instance from the specified Google <see cref="EventDateTime" /> instance
        /// </summary>
        public static DateTime ToDateTime(this EventDateTime eventDateTime)
        {
            if (eventDateTime.DateTime != null)
            {
                return eventDateTime.DateTime.Value;
            }
            if (!String.IsNullOrEmpty(eventDateTime.Date))
            {
                return DateTime.Parse(eventDateTime.Date);
            }
            throw new NotSupportedException("Invalid EventDateTime");
        }
    }
}