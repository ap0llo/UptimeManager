// -----------------------------------------------------------------------------------------------------------
//  Copyright (c) 2015-2017, Andreas Grünwald
//  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// -----------------------------------------------------------------------------------------------------------

using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using NLog;
using UptimeManager.Configuration.Commands;

namespace UptimeManager.Core
{
    /// <summary>
    /// CommandVisitor that executes the specified commands used by SingleDeviceUptimeManagementDaemon
    /// </summary>
    public class CommandExecutor : ICommandVisitor<bool>
    {
        readonly Logger m_Logger = LogManager.GetCurrentClassLogger();


        public bool Visit(PingCommandSpecification pingCommand)
        {
            m_Logger.Info("Pinging {0}", pingCommand.Address);

            var ping = new Ping();
            
            try
            {                
                var reply = ping.SendPingAsync(pingCommand.Address).Result;
                return reply?.Status == IPStatus.Success;
            }
            catch (PingException)
            {
            }
            catch (SocketException)
            {
            }

            return false;
        }

        public bool Visit(ShellCommandSpecification shellCommand)
        {
            m_Logger.Info("Starting '{0}' with arguments '{1}'", shellCommand.ProgramName, shellCommand.Arguments);

            var process = Process.Start(shellCommand.ProgramName, shellCommand.Arguments);

            if (process == null)
            {
                var message = $"Could not launch process for {shellCommand.ProgramName}";
                m_Logger.Error(message);
                throw new CommandExecutionException(message);
            }

            process.WaitForExit();

            m_Logger.Info("Program exited with exit code " + process.ExitCode);

            return process.ExitCode == shellCommand.ExpectedReturnCode;
        }

        public bool Visit(NopCommandSpecification nopCommand) => true; //nop command is always successful        
    }
}