// // -----------------------------------------------------------------------------------------------------------
// //  Copyright (c) 2015-2016, Andreas Grünwald
// //  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// // -----------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UptimeManager.Configuration.Commands;
using Xunit;

namespace UptimeManager.Configuration.Xml
{
    public class XmlConfigurationReaderTest
    {
        readonly XmlConfigurationReader m_Instance;

        public XmlConfigurationReaderTest()
        {
            m_Instance = new XmlConfigurationReader();    
        }


        [Fact]
        public void Empty_Configuration_is_valid_configuration_file()
        {
            var configuration = m_Instance.ReadConfiguration(GetTestDataFilePath("Empty.xml"));
            Assert.Empty(configuration.Devices);
        }

        [Fact]
        public void FilePath_property_is_set_correctly()
        {
            var filePath = GetTestDataFilePath("Empty.xml");
            var configuration = m_Instance.ReadConfiguration(filePath);
            Assert.Equal(filePath, configuration.FilePath);
        }

        [Fact]
        public void Missing_Version_attribute_causes_ConfigurationException()
        {
            Assert.Throws<ConfigurationException>(() => m_Instance.ReadConfiguration(GetTestDataFilePath("MissingVersionAttribute.xml")));
        }
        
        [Fact]
        public void Incompatible_Version_attribute_causes_ConfigurationException()
        {
            Assert.Throws<ConfigurationException>(() => m_Instance.ReadConfiguration(GetTestDataFilePath("IncompatibleVersion.xml")));
        }

        [Fact]
        public void Configuration_is_read_correctly()
        {
            var configuration = m_Instance.ReadConfiguration(GetTestDataFilePath("SingleDevice.xml"));

            Assert.Single(configuration.Devices);

            var device = configuration.Devices.Single();
            Assert.Equal("Device1", device.Name);

            Assert.NotNull(device.IsRunningCommand);
            Assert.NotNull(device.StartCommand);
            Assert.NotNull(device.StopCommand);

            Assert.Equal(new TimeSpan(0,0,5, 0), device.UptimeBufferInterval);

            Assert.Equal("Calendar1", device.UptimeCalendarName);
            Assert.NotNull(device.CalendarProviderSettingsDirectory);
            Assert.Equal(CalendarType.GoogleCalendar, device.UptimeCalendarType);

        }

        [Fact]
        public void Relative_CalendarProviderSettingsDirectory_path_is_interpreted_correctly()
        {
            var configuration = m_Instance.ReadConfiguration(GetTestDataFilePath("SettingsDirectoryPaths.xml"));        
            var devices = configuration.Devices.ToArray();

            var dir = Path.GetDirectoryName(configuration.FilePath);

            var path1 = devices[0].CalendarProviderSettingsDirectory;
            Assert.Equal(Path.Combine(dir, "dir"), path1);


            var path2 = devices[1].CalendarProviderSettingsDirectory;
            Assert.Equal(Path.Combine(Path.GetDirectoryName(dir), "dir"), path2);

            var path3 = devices[2].CalendarProviderSettingsDirectory;
            Assert.Equal("C:\\dir", path3);

        }
        
        [Fact]
        public void Missing_commands_are_filled_with_NopCommaned()
        {
            var configuration = m_Instance.ReadConfiguration(GetTestDataFilePath("NoCommands.xml"));

            Assert.Single(configuration.Devices);

            var device = configuration.Devices.Single();
            Assert.Equal("Device1", device.Name);

            Assert.IsType<NopCommandSpecification>(device.IsRunningCommand);
            Assert.IsType<NopCommandSpecification>(device.StartCommand);
            Assert.IsType<NopCommandSpecification>(device.StopCommand);
        }

        [Fact]
        public void UptimeBufferInterval_is_set_to_default_value_if_missing_from_configuration()
        {
            var configuration = m_Instance.ReadConfiguration(GetTestDataFilePath("DefaultUptimeBufferInterval.xml"));

            Assert.Single(configuration.Devices);

            var device = configuration.Devices.Single();
            Assert.Equal(new TimeSpan(0, 10, 0), device.UptimeBufferInterval);

        }



        public string GetTestDataFilePath(string fileName)
        {
#if NETSTANDARD
            return Path.Combine(Directory.GetCurrentDirectory(), "_TestData", fileName);
#else
            return Path.Combine(Environment.CurrentDirectory, "_TestData", fileName);
#endif
        }


    }
}