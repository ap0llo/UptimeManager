// -----------------------------------------------------------------------------------------------------------
//  Copyright (c) 2015-2017, Andreas Grünwald
//  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// -----------------------------------------------------------------------------------------------------------


namespace UptimeManager.Configuration.Commands
{
    /// <summary>
    /// Interface for command executor. There is a execute method for every type of command specification
    /// The Visit() method of <see cref="ICommandSpecification" /> accepts a argument of this type and calls the appropriate
    /// Visit() method on it
    /// This way the specification and implementation of commands can be separated and the configuration for these commands
    /// stay within the Configuration assembly
    /// Implementation of the visitor pattern
    /// </summary>
    public interface ICommandVisitor<T>
    {
        T Visit(PingCommandSpecification pingCommand);

        T Visit(ShellCommandSpecification shellCommand);

        T Visit(NopCommandSpecification nopCommand);
    }
}