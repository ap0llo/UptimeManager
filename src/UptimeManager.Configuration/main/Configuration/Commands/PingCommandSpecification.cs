// // -----------------------------------------------------------------------------------------------------------
// //  Copyright (c) 2015-2016, Andreas Grünwald
// //  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// // -----------------------------------------------------------------------------------------------------------

using System;

namespace UptimeManager.Configuration.Commands
{
    public class PingCommandSpecification : ICommandSpecification
    {
        public string Address { get; }



        public PingCommandSpecification(string address)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }
            this.Address = address;
        }


        public T Accept<T>(ICommandVisitor<T> executor) => executor.Visit(this);
    }
}