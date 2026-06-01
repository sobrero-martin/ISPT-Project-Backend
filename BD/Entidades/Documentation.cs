using System;
using System.Collections.Generic;
using System.Text;

namespace BD.Entidades
{
    public class Documentation : BaseEntity
    {
        public long PersonId { get; set; }
        public Person Person { get; set; }
        
        // One-Time deliverable documents
        public bool DNI { get; set; }
        public bool Picture { get; set; }
        public bool BirthdateDocument { get; set; }
        
        // Deliverable documents
        public bool CUS { get; set; }
        public bool CNIRDS { get; set; }
        public bool CDA { get; set; }
        public bool Cooperative { get; set; }
        public bool CNIRDAM { get; set; }
        public bool CAP { get; set; }
        public bool CDS { get; set; }
        
        public DateTime Date { get; set; }
    }
}
