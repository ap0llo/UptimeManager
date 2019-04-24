// -----------------------------------------------------------------------------------------------------------
//  Copyright (c) 2015-2017, Andreas Grünwald
//  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// -----------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using UptimeManager.Calendar;
using UptimeManager.Configuration;
using UptimeManager.Core;

namespace UptimeManager.Daemon
{
    /// <summary>
    /// Implementation of uptime management daemon for a single device 
    /// The daemon constantly monitors the device and issues start/stop commands
    /// to it according to the device's uptime requests
    /// </summary>
    public class SingleDeviceUptimeManagementDaemon : IDisposable
    {
        const int s_FaultCountMax = 20;
        readonly CancellationTokenSource m_CancellationTokenSource = new CancellationTokenSource();

        readonly CommandExecutor m_CommandExecutor = new CommandExecutor();
        readonly Logger m_Logger = LogManager.GetCurrentClassLogger();
        readonly IDeviceConfiguration m_Device;
        readonly UptimeManagement m_UptimeRequestProvider;

        bool m_Disposed;


        /// <summary>
        /// Initializes a new instance of SingleDeviceUptimeManagementDaemon
        /// </summary>
        public SingleDeviceUptimeManagementDaemon(IDeviceConfiguration device, ICalendarFactory calendarFactory)
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
            this.m_UptimeRequestProvider = new UptimeManagement(m_Device, calendarFactory);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }



        /// <summary>
        /// Checks the state of the device and issues the appropriate commands if necessary
        /// </summary>
        void CheckState()
        {
            m_Logger.Info($"Checking state for device '{m_Device.Name}'");


            m_Logger.Info($"Getting calendar '{m_Device.UptimeCalendarName}'");

            var activeRequests = m_UptimeRequestProvider.GetActiveUptimeRequests().ToList();

            var expectedState = activeRequests.Any() ? DeviceState.Running : DeviceState.Off;

            m_Logger.Info($"Determined expected state for device '{m_Device.Name}': '{expectedState}'");


            m_Logger.Info($"Checking state of device '{m_Device.Name}'");

            var currentState = m_Device.IsRunningCommand.Accept(m_CommandExecutor) ? DeviceState.Running : DeviceState.Off;


            if (expectedState != currentState)
            {
                if (currentState == DeviceState.Off)
                {
                    m_Logger.Info($"Starting {m_Device.Name}");
                    m_Device.StartCommand.Accept(m_CommandExecutor);
                }
                else
                {
                    m_Logger.Info($"Shutting down {m_Device.Name}");
                    m_Device.StopCommand.Accept(m_CommandExecutor);
                }
            }
            else
            {
                m_Logger.Info($"No actions need to be taken for devie '{m_Device.Name}', device is already in expected state");
            }

            m_Logger.Info($"Checking state for device '{m_Device.Name}' complete");
        }


        /// <summary>
        /// Starts a new task monitoring the device
        /// </summary>
        public Task Start()
        {
            m_Logger.Info($"Starting task for Device '{m_Device.Name}'");

            var task = Task.Factory.StartNew(() =>
            {
                //count number of exceptions caught, will be reset once a run does not cause an error
                var faultCount = 0;

                while (true)
                {
                    try
                    {
                        CheckState();

                        //no exception thrown by CheckState() => reset faultCount
                        faultCount = 0;
                    }
                    catch (CalendarException ex)
                    {
                        //increment faultCount
                        faultCount++;

                        //if faultCount exceeds maximum number of allowed faults, do not ignore exception
                        if (faultCount > s_FaultCountMax)
                        {
                            throw new UptimeManagementException($"Maximum number of faults exceeded, faultCount ={faultCount}", ex);
                        }
                        //Ignore
                        m_Logger.Error(ex, "Caught CalendarException");
                    }
                    catch (Exception ex)
                    {
                        m_Logger.Debug(ex, "Unhandled exception in SingleDeviceUptimeManagementDaemon loop");
                        throw;
                    }

                    Task.Delay(60000, m_CancellationTokenSource.Token).Wait();
                }
            }, m_CancellationTokenSource.Token);

            return task;
        }

        /// <summary>
        /// Stops all device monitoring tasks created by this instance of <see cref="SingleDeviceUptimeManagementDaemon"/>
        /// </summary>
        public void Stop() => m_CancellationTokenSource.Cancel();

        protected virtual void Dispose(bool disposing)
        {
            if (m_Disposed)
            {
                return;
            }

            if (disposing)
            {
                m_CancellationTokenSource.Dispose();
            }
            m_Disposed = true;
        }

        ~SingleDeviceUptimeManagementDaemon()
        {
            Dispose(false);
        }
    }
}