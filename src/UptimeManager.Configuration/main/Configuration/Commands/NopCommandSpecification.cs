// -----------------------------------------------------------------------------------------------------------
//  Copyright (c) 2015-2017, Andreas Grünwald
//  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// -----------------------------------------------------------------------------------------------------------

namespace UptimeManager.Configuration.Commands
{
    public class NopCommandSpecification : ICommandSpecification
    {
        public T Accept<T>(ICommandVisitor<T> executor) => executor.Visit(this);
    }
}