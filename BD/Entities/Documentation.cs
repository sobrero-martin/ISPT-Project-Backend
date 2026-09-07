using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace BD.Entidades
{
    [Index(nameof(FileId), nameof(Date), IsUnique = true)]
    public class Documentation : BaseEntity
    {
        public long FileId { get; set; }
        public File File { get; set; }
        
        // Deliverable documents
        public bool CUS { get; set; }
        public bool CNIRDS { get; set; }
        public bool CDA { get; set; }
        public bool Cooperative { get; set; }
        
        public int Date { get; set; }
    }
}
