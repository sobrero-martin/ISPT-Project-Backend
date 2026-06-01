using System;
using System.Collections.Generic;
using System.Text;

namespace BD.Entidades
{
    public class Contact : BaseEntity
    {
        public long PersonId { get; set; }
        public Person Person { get; set; }
        
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        
        public string EmergencyNumber { get; set; }
        public string ContactNameEmergency { get; set; }
    }
}