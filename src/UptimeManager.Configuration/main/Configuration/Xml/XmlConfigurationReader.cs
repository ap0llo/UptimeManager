// // -----------------------------------------------------------------------------------------------------------
// //  Copyright (c) 2015-2016, Andreas Grünwald
// //  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// // -----------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using UptimeManager.Configuration.Commands;

namespace UptimeManager.Configuration.Xml
{
    /// <summary>
    /// Configuration reader for xml config files
    /// </summary>
    class XmlConfigurationReader : IConfigurationReader
    {

        static readonly string[] s_SupportedConfigurationVersions = { "1.0" };

        const string s_ConfigurationSchema = "UptimeManager.Configuration.Xml.ConfigurationSchema.xsd";



        public IUptimeManagerConfiguration ReadConfiguration(string filePath)
        {
            var absoluteFilePath = Path.GetFullPath(filePath);
            var document = XDocument.Load(absoluteFilePath);

            document.Validate(GetConfigurationSchema(), (sender, args) =>
                {
                    throw new ConfigurationException("Invalid configuration-file: " + args.Message, args.Exception);
                });

            var versionString = document.Root.RequireAttributeValue(XmlAttributeNames.Version);
            EnsureVersionIsSupported(versionString);

            var result = new ImmutableUptimeManagerConfiguration(
                    absoluteFilePath,
                    document.Descendants(XmlNames.Device)
                            .Select(element => ReadDevice(element, Path.GetDirectoryName(filePath)))
                );

            return result;
        }


        IDeviceConfiguration ReadDevice(XElement deviceElement, string configDirectory)
        {
            var uptimeBufferInterval = new TimeSpan(0, 10, 0);

            if (deviceElement.Element(XmlNames.UptimeBufferInterval) != null)
            {
                uptimeBufferInterval = ReadTimeSpan(deviceElement.Element(XmlNames.UptimeBufferInterval));
            }

            //command specifications are optional beginning with file formar version 0.3
            //if no command has been specified, use NopCommand, so command can be called without checking for null
            var isRunningCommand = deviceElement.Element(XmlNames.IsRunningCommand) != null
                ? ReadCommand(deviceElement.Element(XmlNames.IsRunningCommand))
                : new NopCommandSpecification();
            var startCommand = deviceElement.Element(XmlNames.StartCommand) != null
                ? ReadCommand(deviceElement.Element(XmlNames.StartCommand))
                : new NopCommandSpecification();
            var stopCommand = deviceElement.Element(XmlNames.StopCommand) != null
                ? ReadCommand(deviceElement.Element(XmlNames.StopCommand))
                : new NopCommandSpecification();

            var device = new ImmutableDeviceConfiguration(
                    name: deviceElement.RequireAttributeValue(XmlAttributeNames.Name),
                    uptimeCalendarType: GetCalendarType(deviceElement),
                    calendarProviderSettingsName: GetCalendarSettingsDirectory(deviceElement, configDirectory),
                    uptimeCalendarName: GetCalendarName(deviceElement),
                    uptimeBufferInterval: uptimeBufferInterval,
                    isRunningCommand: isRunningCommand,
                    startCommand: startCommand,
                    stopCommand: stopCommand);

            return device;
        }

        CalendarType ReadCalendarType(string calendarTypeString)
        {
            CalendarType calendarType;
            if (!Enum.TryParse(calendarTypeString, out calendarType))
            {
                throw new ConfigurationException("Could not parse value '{0}' as CalendarType");
            }
            return calendarType;
        }

        CalendarType GetCalendarType(XElement deviceElement)
        {            
            return ReadCalendarType(GetCalendarElement(deviceElement).RequireAttributeValue(XmlAttributeNames.Type));
        }

        string GetCalendarSettingsDirectory(XElement deviceElement, string configurationDirectory)
        {
            var value = GetCalendarElement(deviceElement).RequireAttributeValue(XmlAttributeNames.SettingsDirectory);

            if (Path.IsPathRooted(value))
            {
                return value;
            }
            else
            {
                return Path.GetFullPath(Path.Combine(configurationDirectory, value));
            }
        }


        string GetCalendarName(XElement deviceElement)
        {
            return GetCalendarElement(deviceElement).RequireAttributeValue(XmlAttributeNames.Name);
        }


        XElement GetCalendarElement(XElement deviceElement)
        {
            var element = deviceElement
                ?.Element(XmlNames.UptimeProviders)
                ?.Element(XmlNames.Calendar);

            return element;
        }

        ICommandSpecification ReadCommand(XElement commandElement)
        {
            if (commandElement.Elements().Count() != 1)
            {
                throw new ConfigurationException("Multiple child nodes in command element");
            }

            var concreteCommandElement = commandElement.Elements().First();


            if (concreteCommandElement.Name == XmlNames.PingCommand)
            {
                return ReadPingCommand(concreteCommandElement);
            }
            else if (concreteCommandElement.Name == XmlNames.ShellCommand)
            {
                return ReadShellCommand(concreteCommandElement);
            }
            else if (concreteCommandElement.Name == XmlNames.NopCommand)
            {
                return ReadNopCommand(concreteCommandElement);
            }
            else
            {
                throw new ConfigurationException("Unknown command type: " + concreteCommandElement.Name.LocalName);
            }

        }

        ICommandSpecification ReadPingCommand(XElement pingCommandElement)
        {
            var address = pingCommandElement.RequireAttributeValue(XmlAttributeNames.Address);
            return new PingCommandSpecification(address);
        }

        ICommandSpecification ReadShellCommand(XElement shellCommandElement)
        {

            var programName = shellCommandElement.RequireAttributeValue(XmlAttributeNames.ProgramName);
            var arguments = shellCommandElement.RequireAttributeValue(XmlAttributeNames.Arguments);
            var returnCode = 0;

            //parse expected return code (optional)
            var returnCodeAttribute = shellCommandElement.Attribute(XmlAttributeNames.ExpectedReturnCode);
            if (returnCodeAttribute != null)
            {
                if (!int.TryParse(returnCodeAttribute.Value, out returnCode))
                {
                    throw new ConfigurationException(String.Format("Could not parse value '{0}' as integer", returnCodeAttribute.Value));
                }
            }

            return new ShellCommandSpecification(programName, arguments, returnCode);
        }

        ICommandSpecification ReadNopCommand(XElement nopCommandElement) => new NopCommandSpecification();

        XmlSchemaSet GetConfigurationSchema()
        {
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(s_ConfigurationSchema))
            {
                var schemaSet = new XmlSchemaSet();

                schemaSet.Add(null, XmlReader.Create(stream));

                return schemaSet;
            }
        }

        void EnsureVersionIsSupported(string versionString)
        {
            if (!s_SupportedConfigurationVersions.Contains(versionString))
            {
                throw new ConfigurationException(String.Format("Unsupported configuration version: '{0}'", versionString));
            }
        }

        TimeSpan ReadTimeSpan(XElement timeSpanElement)
        {
            var hoursAttribute = timeSpanElement.Attribute(XmlAttributeNames.Hours);
            var minutesAttribute = timeSpanElement.Attribute(XmlAttributeNames.Minutes);
            var secondsAttribute = timeSpanElement.Attribute(XmlAttributeNames.Seconds);

            int hours = hoursAttribute != null ? int.Parse(hoursAttribute.Value) : 0;
            int minutes = minutesAttribute != null ? int.Parse(minutesAttribute.Value) : 0;
            int seconds = secondsAttribute != null ? int.Parse(secondsAttribute.Value) : 0;

            return new TimeSpan(hours, minutes, seconds);
        }



    }
}
