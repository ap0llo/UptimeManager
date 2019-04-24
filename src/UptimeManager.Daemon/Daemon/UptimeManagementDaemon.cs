using NLog;
using System;
using System.Linq;
using System.Threading.Tasks;
using UptimeManager.Calendar;
using UptimeManager.Configuration;

namespace UptimeManager.Daemon
{
    /// <summary>
    /// Daemon that runs uptime management for all devices
    /// </summary>
    public class UptimeManagementDaemon
    {
        
        readonly Logger m_Logger = LogManager.GetCurrentClassLogger();

        private readonly ConfigurationReaderFactory m_ConfigurationReaderFactory = new ConfigurationReaderFactory();    
        private readonly CalendarFactory m_CalendarFactory = new CalendarFactory();        

        
        /// <summary>
        /// Runs the daemon.
        /// </summary>
        /// <remarks>
        /// This method will not return
        /// </remarks>        
        public void Run(string settingsFilePath)
        {
            m_Logger.Info("Loading configuration from '{0}'", settingsFilePath);

            // Read Configuration
            var configreader = m_ConfigurationReaderFactory.GetConfigurationReader(ConfigurationType.Xml);
            var config = configreader.ReadConfiguration(settingsFilePath);


            m_Logger.Info("Found {0} devices in configuration", config.Devices.Count());

            var daemons = config.Devices.Select(d => new SingleDeviceUptimeManagementDaemon(d, m_CalendarFactory));

            //run uptime manager for all devices
            var tasks = daemons.Select(uptimeManager => uptimeManager.Start()).ToArray();

            //wait for all tasks to complete (they should run indefinitely)
            //WaitAny should suffice as all tasks are supposed to run indefinitely
            //using Any we get a exception if on of the tasks crashed
            //using All we would not get a error if just a single tasks crashes and the others continue to run
            var completedTaskIndex = Task.WaitAny(tasks);            

            m_Logger.Debug("Left Wait() in UptimeManagementRunner. This should not happen");

            m_Logger.Debug("Task {0} has completed, IsCanceled = {1}", completedTaskIndex, tasks[completedTaskIndex].IsCanceled);

            throw new InvalidOperationException("Left Wait() in UptimeManagementRunner. This should not happen");

        }

      
    }
}
