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

namespace UptimeManager.Core.Client
{
    /// <summary>
    /// Client for uptime management. Used by clients to retrieve and create uptime requests
    /// </summary>
    public class UptimeManagementClient
    {
        readonly UptimeManagement m_UptimeManagement;



        /// <summary>
        /// The name of the device being managed by this client
        /// </summary>
        public string DeviceName { get; }

        /// <summary>
        /// All uptime requests active at the time of the last update
        /// The list also includes requests that will become active before others in the same uptime expire
        /// </summary>
        public IEnumerable<ICalendarEvent> CurrentUptimeRequests { get; private set; }

        /// <summary>
        /// A single "virtual" uptime request spanning the entire time in the current uptime
        /// (Calculated from all the requests in CurrentUptimeRequests)
        /// </summary>
        public ICalendarEvent CombinedCurrentUptimeRequest { get; private set; }

        /// <summary>
        /// All uptime request for the next uptime (the next time the device starts after the current uptime has expired)
        /// The list also includes requests that will become active before others in the same uptime expire
        /// </summary>
        public IEnumerable<ICalendarEvent> NextUptimeRequests { get; private set; }

        /// <summary>
        /// A single "virtual" uptime request spanning the entire next uptime
        /// (Calculated from all the requests in NextUptimeRequests)
        /// </summary>
        public ICalendarEvent CombinedNextUptimeRequest { get; private set; }



        /// <summary>
        /// Initializes a new instance of UptimeManagementClient
        /// </summary>
        /// <param name="device">Configuration for the device being managed</param>
        /// <param name="calendarFactory">CalendarFactory to instantiate instances of calendar clients</param>
        public UptimeManagementClient(IDeviceConfiguration device, ICalendarFactory calendarFactory)
        {
            if (device == null)
            {
                throw new ArgumentNullException(nameof(device));
            }

            if (calendarFactory == null)
            {
                throw new ArgumentNullException(nameof(calendarFactory));
            }

            this.DeviceName = device.Name;
            this.m_UptimeManagement = new UptimeManagement(device, calendarFactory);
        }



        /// <summary>
        /// Updates Requests for the current and next uptime
        /// </summary>
        public void Update()
        {
            //get current uptime requests
            this.CurrentUptimeRequests = m_UptimeManagement.GetAllRequestsForUptime(false).ToList();
            this.CombinedCurrentUptimeRequest = CombineUptimeRequests(CurrentUptimeRequests);

            //determine when the current uptime ends
            var currentUptimeEnd = this.CurrentUptimeRequests.Any()
                ? this.CurrentUptimeRequests.Max(request => request.End)
                : DateTime.Now;

            //get the start of the next uptime after the end of the current uptime

            var nextUptimeStart = m_UptimeManagement.GetFirstUptimeRequest(currentUptimeEnd).Start;

            //get all uptime requests for the uptime beginning at nextUptimeStart            
            this.NextUptimeRequests = m_UptimeManagement.GetAllRequestsForUptime(nextUptimeStart, false);
            this.CombinedNextUptimeRequest = CombineUptimeRequests(this.NextUptimeRequests);
        }



        /// <summary>
        /// Combines the specified uptime requests to a single "virtual" uptime request
        /// </summary>
        /// <returns>
        /// Returns a combined uptime request. Returns null if specified list of uptime requests is empty
        /// </returns>
        ICalendarEvent CombineUptimeRequests(IEnumerable<ICalendarEvent> uptimeRequests)
        {
            if (uptimeRequests == null)
            {
                throw new ArgumentNullException(nameof(uptimeRequests));
            }

            uptimeRequests = uptimeRequests.ToList();

            if (uptimeRequests.Any())
            {
                return new CalendarEvent(null, null, false)
                {
                    Name = "CombinedUptimeRequest",
                    Start = uptimeRequests.Min(request => request.Start),
                    End = uptimeRequests.Max(request => request.End)
                };
            }
            return null;
        }
    }
}