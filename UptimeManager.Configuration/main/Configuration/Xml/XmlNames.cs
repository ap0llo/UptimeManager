// // -----------------------------------------------------------------------------------------------------------
// //  Copyright (c) 2015-2016, Andreas Grünwald
// //  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// // -----------------------------------------------------------------------------------------------------------
using System.Xml.Linq;

namespace UptimeManager.Configuration.Xml
{
    /// <summary>
    /// Xml element names for reading xml configuration files
    /// </summary>
    class XmlNames
    {

        private const string s_Namespace = "http://grynwald.net/schemas/2016/UptimeManager/v1/Configuration/";

        public static readonly XName UptimeManagerConfiguration = ExpandName("UptimeManagerConfiguration");
        public static readonly XName Device = ExpandName("Device");
        public static readonly XName IsRunningCommand = ExpandName("IsRunningCommand");
        public static readonly XName StartCommand = ExpandName("StartCommand");
        public static readonly XName StopCommand = ExpandName("StopCommand");
        public static readonly XName PingCommand = ExpandName("PingCommand");
        public static readonly XName ShellCommand = ExpandName("ShellCommand");
        public static readonly XName NopCommand = ExpandName("NopCommand");
        public static readonly XName UptimeBufferInterval = ExpandName("UptimeBufferInterval");

        public static readonly XName UptimeProviders = ExpandName("UptimeProviders");
        public static readonly XName Calendar = ExpandName("Calendar");
        
        static XName ExpandName(string localName) => XName.Get(localName, s_Namespace);


        public static XNamespace GetNamespace() => s_Namespace;
        
    }
}
