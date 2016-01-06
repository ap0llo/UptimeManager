// // -----------------------------------------------------------------------------------------------------------
// //  Copyright (c) 2015-2016, Andreas Grünwald
// //  Licensed under the MIT License. See LICENSE.txt file in the project root for full license information.  
// // -----------------------------------------------------------------------------------------------------------

namespace UptimeManager.Calendar.Model
{
    class ImmutableCalendarUser : ICalendarUser
    {
        
        public string Name
        {
            get;            
        }

        public string Email
        {
            get;            
        }
        

        public ImmutableCalendarUser(string name, string email)
        {
            this.Name = name;
            this.Email = email;
        }
       
    }
}
