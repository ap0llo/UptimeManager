// -----------------------------------------------------------------------------------------------------------
//  Copyright (c) 2015-2017, Andreas Grünwald
//  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// -----------------------------------------------------------------------------------------------------------

namespace UptimeManager.Configuration.Commands
{
    /// <summary>
    /// Interface for different specifications of commands that can be specified in a configuration file
    /// </summary>
    public interface ICommandSpecification
    {
        T Accept<T>(ICommandVisitor<T> executor);
    }
}