using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using NLog;

namespace UptimeManager.Daemon
{
    static class Program
    {
        const string s_PIdFileName = "UptimeManager.pid";
        static readonly TimeSpan s_AutoRelaunchMinRuntime = new TimeSpan(0, 5, 0); // 5 minutes

        static DateTime s_ProcessStartTime;
        static readonly Logger s_Logger = LogManager.GetCurrentClassLogger();


        public static void Main(string[] args)
        {
            try
            {
                s_Logger.Info($"Starting UptimeManager, Version {Assembly.GetExecutingAssembly().GetName().Version}");

                OnStartup();

                var uptimeManager = new UptimeManagementDaemon();
                uptimeManager.Run(args[0]);
            }
            catch (Exception ex)
            {
                // handle unhandled exception

                s_Logger.Error("Unhandled exception in main()");
                s_Logger.Fatal(ex);

                var runtime = DateTime.Now - s_ProcessStartTime;
                s_Logger.Info("Program was running for " + runtime);

                // if the program was running at least the specified run time (so it did not crash right away after startup), relaunch the program
                if (runtime > s_AutoRelaunchMinRuntime)
                {
                    s_Logger.Info("Process was running longer than the minimum timespan for auto relaunch. Relaunching process");
                    var programFile = Assembly.GetExecutingAssembly().Location;
                    Process.Start(programFile, $"\"{args[0]}\"");
                }
            }
            finally
            {
                s_Logger.Info("Application exiting");
            }
        }


        static void OnStartup()
        {
            //save application startup time (necessary to decide whether to automatically relaunch the process on crash)
            s_ProcessStartTime = DateTime.Now;

            //write PID file
            var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var fileName = Path.Combine(dir, s_PIdFileName);
            using (var writer = new StreamWriter(File.Open(fileName, FileMode.Create)))
            {
                writer.Write(Process.GetCurrentProcess().Id);
            }
        }
    }
}