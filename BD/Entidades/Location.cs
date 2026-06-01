using System;
using System.Collections.Generic;
using System.Text;

namespace BD.Entidades
{
    public class Location : BaseEntity
    {
        public long PersonId { get; set; }
        public Person Person { get; set; }
        
        public string Country { get; set; }
        public string Province { get; set; }
        public string Department { get; set; }
        public string Address { get; set; }
    }
}
