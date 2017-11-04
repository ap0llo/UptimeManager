// -----------------------------------------------------------------------------------------------------------
//  Copyright (c) 2015-2017, Andreas Grünwald
//  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// -----------------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Management.Automation;
using System.Reflection;

namespace UptimeManager.Client.Powershell.Initialization
{
    /// <summary>
    /// Powershell module initializer that ensures that the assembly System.Net.Http.Primitives required by Google calendar
    /// client is loaded correctly
    /// </summary>
    public class ModuleAssemblyLoader : IModuleAssemblyInitializer
    {

        const string s_SystemNetHttpPrimitives = "System.Net.Http.Primitives";
        const string s_Dll = ".dll";


        public void OnImport()
        {
            //register for the assembly resolve event            
            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolveEventHandler;
        }


        static Assembly AssemblyResolveEventHandler(object sender, ResolveEventArgs args)
        {
            //parse the name of the requested assembly
            var assemblyName = new AssemblyName(args.Name);

            //for System.Net.Http.Primitives use the assembly located next to the cmdlet assembly
            if (assemblyName.Name == s_SystemNetHttpPrimitives)
            {
                var dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                var assembly = Assembly.LoadFile(Path.Combine(dir, s_SystemNetHttpPrimitives + s_Dll));
                return assembly;
            }

            //do not change behavior for other assemblies
            return null;
        }
    }
}