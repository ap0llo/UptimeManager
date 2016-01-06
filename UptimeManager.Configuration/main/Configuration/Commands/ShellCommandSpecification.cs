// // -----------------------------------------------------------------------------------------------------------
// //  Copyright (c) 2015-2016, Andreas Grünwald
// //  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// // -----------------------------------------------------------------------------------------------------------

namespace UptimeManager.Configuration.Commands
{
    public class ShellCommandSpecification : ICommandSpecification
    {
        public string ProgramName { get; }

        public string Arguments { get; }

        public int ExpectedReturnCode { get; }


        public ShellCommandSpecification(string programName, string arguments, int expectedReturnCode)
        {
            this.ProgramName = programName;
            this.Arguments = arguments;
            this.ExpectedReturnCode = expectedReturnCode;
        }


        public T Accept<T>(ICommandVisitor<T> executor) => executor.Visit(this);
    }
}