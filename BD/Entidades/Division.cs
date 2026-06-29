using System;
using System.Collections.Generic;
using System.Text;

namespace BD.Entidades
{
    public class Division : BaseEntity
    {
        public long DivisionTemplateId { get; set; }
        public DivisionTemplate? DivisionTemplate { get; set; }

        public long SchoolYearId { get; set; }
        public SchoolYear? SchoolYear { get; set; }

        public required string DivisionState { get; set; }

        public Schedule? Schedule { get; set; }
    }
}
