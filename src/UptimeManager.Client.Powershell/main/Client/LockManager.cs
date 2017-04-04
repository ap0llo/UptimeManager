// // -----------------------------------------------------------------------------------------------------------
// //  Copyright (c) 2015-2016, Andreas Grünwald
// //  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// // -----------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UptimeManager.Calendar;
using UptimeManager.Calendar.Model;
using UptimeManager.Configuration;

namespace UptimeManager.Client
{
    class LockManager
    {
        /// <summary>
        /// Interval in which the vent is updated (event is prolonged by this duration)
        /// </summary>
        static readonly TimeSpan s_LockInterval = new TimeSpan(hours: 0, minutes: 10, seconds: 0);

        static readonly IDictionary<string, LockManager> s_Instances = new Dictionary<string, LockManager>(StringComparer.InvariantCultureIgnoreCase); 


        public static LockManager GetInstance(IUptimeManagerConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            if (!s_Instances.ContainsKey(configuration.FilePath))
            {
                var instance = new LockManager(configuration);
                s_Instances.Add(configuration.FilePath, instance);
                return instance;
            }
            else
            {
                return s_Instances[configuration.FilePath];                
            }
        }


        readonly IUptimeManagerConfiguration m_Configuration;
        readonly Dictionary<IDeviceConfiguration, CancellationTokenSource> m_CancellationTokens = new Dictionary<IDeviceConfiguration, CancellationTokenSource>();


        /// <summary>
        /// Initializes a new instance of LockManager
        /// </summary>
        private LockManager(IUptimeManagerConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            m_Configuration = configuration;
        }



        /// <summary>
        /// Prevents the shutdown of the specified device by creating a event in the device's uptime calendar.
        /// This event is prolonged automatically on a regular basis until UnlockShutdown() is called.
        /// </summary>
        public void LockShutdown(string deviceName, string comment)
        {
            var device = GetDevice(deviceName);

            if (m_CancellationTokens.ContainsKey(device))
            {
                throw new LockManagerException($"Shutdown for the server'{deviceName}' has already been locked");
            }

            var cancellationTokenSource = new CancellationTokenSource();
            m_CancellationTokens.Add(device, cancellationTokenSource);

            var calendar = GetCalendar(device);
            var lockEvent = CreateLockEvent(calendar, comment);

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    try
                    {                       
                        //download updated calendar event (might have been updated since we last changed it)
                        lockEvent = calendar.GetEvent(lockEvent.Id);
                        if (lockEvent.End < DateTime.Now)
                        {
                            //the lock event has expired (client which is supposed to extend the lock might have been hibernated)
                            // => create a new lock event
                            lockEvent = CreateLockEvent(calendar, comment);
                        }
                        else
                        {
                            //extend the lock event by the lock interval
                            lockEvent.End = lockEvent.End + s_LockInterval;
                            lockEvent = calendar.UpdateEvent(lockEvent);
                        }
                        Task.Delay((int) s_LockInterval.TotalMilliseconds, cancellationTokenSource.Token).Wait(cancellationTokenSource.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        //expected exception => task has been canceled
                        lockEvent.End = DateTime.Now;
                        lockEvent = calendar.UpdateEvent(lockEvent);
                        break;
                    }
                }
            }, cancellationTokenSource.Token);
        }

        /// <summary>
        /// Allows shutdown for the specified device by no longer prolonging the event created be LockShutdown()
        /// </summary>
        public void UnlockShutdown(string deviceName)
        {
            var server = GetDevice(deviceName);

            if (!m_CancellationTokens.ContainsKey(server))
            {
                throw new LockManagerException("Shutdown for the specified server has not been locked");
            }

            var tokenSource = m_CancellationTokens[server];
            tokenSource.Cancel();
            m_CancellationTokens.Remove(server);
        }

        
        IDeviceConfiguration GetDevice(string name)
        {
            var server = m_Configuration.Devices.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

            if (server == null)
            {
                throw new LockManagerException($"Device '{name}' not found");
            }

            return server;
        }

        ICalendarEvent CreateLockEvent(ICalendar calendar, string comment)
        {
            var eventName = String.IsNullOrEmpty(comment)
                ? "Shutdown Lock"
                : $"Shutdown Lock '{comment}'";
            try
            {
                return calendar.AddEvent(eventName, DateTime.Now, DateTime.Now + (s_LockInterval + s_LockInterval));
            }
            catch (CalendarException ex)
            {
                throw new LockManagerException("Error accessing calendar", ex);
            }
        }

        ICalendar GetCalendar(IDeviceConfiguration device)
        {
            ICalendar calendar;
            try
            {
                var calendarFactory = new CalendarFactory();
                calendar = calendarFactory.GetCalendar(device.UptimeCalendarType, device.CalendarProviderSettingsDirectory, device.UptimeCalendarName);
            }
            catch (CalendarException ex)
            {
                throw new LockManagerException("Error accessing calendar", ex);
            }
            return calendar;
        }
        
    }
}