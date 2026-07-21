using System;
using System.Collections.Generic;
using System.Text;

namespace BD.Entidades
{
    public class DivisionTemplate : BaseEntity
    {
        public long SubjectId { get; set; }
        public Subject? Subject { get; set; }

        public required string Name { get; set; }

        public string? TemplateState { get; set; } 
    }
}
