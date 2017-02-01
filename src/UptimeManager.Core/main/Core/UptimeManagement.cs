// // -----------------------------------------------------------------------------------------------------------
// //  Copyright (c) 2015-2016, Andreas Grünwald
// //  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// // -----------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using UptimeManager.Calendar;
using UptimeManager.Calendar.Model;
using UptimeManager.Configuration;
using UptimeManager.Core.Client;

namespace UptimeManager.Core
{
    /// <summary>
    /// Abstraction of calendar service for uptime management 
    /// </summary>
    public class UptimeManagement
    {
        readonly ICalendar m_Calendar;
        readonly IDeviceConfiguration m_Device;



        /// <summary>
        /// Initializes a new instance of UptimeManagement
        /// </summary>
        public UptimeManagement(IDeviceConfiguration device, ICalendarFactory calendarFactory)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            if (calendarFactory == null)
            {
                throw new ArgumentNullException(nameof(calendarFactory));
            }

            this.m_Device = device;
            this.m_Calendar = calendarFactory.GetCalendar(m_Device.UptimeCalendarType.ToCalendarCalendarType(),
                m_Device.CalendarProviderSettingsDirectory,
                m_Device.UptimeCalendarName);
        }


        /// <summary>
        /// Gets all currently active uptime requests
        /// </summary>
        /// <param name="includeBufferInterval">
        /// Specifies whether to use the buffer interval specified in the device configuration
        /// </param>
        public IEnumerable<ICalendarEvent> GetActiveUptimeRequests(bool includeBufferInterval = true)
            => GetActiveUptimeRequests(DateTime.Now, includeBufferInterval);


        /// <summary>
        /// Gets all uptime requests active at the specified time
        /// </summary>
        /// <param name="time">
        /// The time for which to get active uptime requests
        /// </param>
        /// <param name="includeBufferInterval">
        /// Specifies whether to use the buffer interval specified in the device configuration
        /// </param>
        public IEnumerable<ICalendarEvent> GetActiveUptimeRequests(DateTime time, bool includeBufferInterval)
        {
            var bufferInterval = includeBufferInterval ? m_Device.UptimeBufferInterval : new TimeSpan(0);

            var events = m_Calendar.GetEvents(time - bufferInterval);
            var activeRequests = events.Where(ev => IsActiveRequest(time, ev)).ToList();
            return activeRequests.ToList();
        }

        /// <summary>
        /// Gets all uptime requests for the current uptime.
        /// This includes both active uptime requests and requests that will become active before any of the other
        /// requests expires
        /// </summary>
        /// <param name="includeBufferInterval">
        /// Specifies whether to use the buffer interval specified in the device configuration
        /// </param>
        public IEnumerable<ICalendarEvent> GetAllRequestsForUptime(bool includeBufferInterval = true)
            => GetAllRequestsForUptime(DateTime.Now, includeBufferInterval);

        /// <summary>
        /// Gets all uptime requests for the uptime at the specified time
        /// Includes both active uptime requests and requests that will become active before any of the other
        /// requests expires
        /// </summary>
        /// <param name="time">
        /// The earliest time for the desired uptime
        /// </param>
        /// <param name="includeBufferInterval">
        /// Specifies whether to use the buffer interval specified in the device configuration
        /// </param>
        public IEnumerable<ICalendarEvent> GetAllRequestsForUptime(DateTime time, bool includeBufferInterval)
        {
            var activeRequests = GetActiveUptimeRequests(time, includeBufferInterval).ToList();

            var bufferInterval = includeBufferInterval ? m_Device.UptimeBufferInterval : new TimeSpan(0);

            if (activeRequests.Any())
            {
                var newEntriesAdded = false;
                do
                {
                    var endMax = activeRequests.Max(ev => ev.End);
                    var candidates = m_Calendar.GetEvents(endMax - bufferInterval);

                    var newEntries = candidates.Where(ev => activeRequests.All(ev2 => ev2.Id != ev.Id))
                        .Where(ev => IsActiveRequest(endMax, ev.Start, ev.End)).ToList();

                    newEntriesAdded = newEntries.Any();
                    activeRequests.AddRange(newEntries);
                }
                while (newEntriesAdded);
            }

            return activeRequests;
        }

        /// <summary>
        /// Gets the first uptime request that will become active after the specified time
        /// </summary>
        public ICalendarEvent GetFirstUptimeRequest(DateTime min)
            => m_Calendar.GetEvents(min).First();


        /// <summary>
        /// Determines whether a uptime request is active at a specified time
        /// </summary>
        /// <param name="now">The time for which to check the activeness of the request</param>
        /// <param name="calendarEvent">The uptime request to check</param>
        bool IsActiveRequest(DateTime now, ICalendarEvent calendarEvent)
            => IsActiveRequest(now, calendarEvent.Start, calendarEvent.End);    

        //internal for unit testing
        /// <summary>
        /// Determines whether a uptime request which the specified start and end time is active at the specified time
        /// </summary>
        /// <remarks>
        /// Visibility is set to internal for the purpose of unit testing (internals of the project are visible to the associated
        /// test project)
        /// </remarks>
        internal bool IsActiveRequest(DateTime now, DateTime start, DateTime end)
            => (start - m_Device.UptimeBufferInterval) <= now && now <= (end + m_Device.UptimeBufferInterval);
    }
}