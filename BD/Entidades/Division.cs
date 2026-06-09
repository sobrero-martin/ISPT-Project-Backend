using System;
using System.Collections.Generic;
using System.Text;

namespace BD.Entidades
{
    public class Division : BaseEntity
    {
        public long SubjectId { get; set; }
        public Subject? Subject { get; set; }

        public long SchoolYearId { get; set; }
        public SchoolYear? SchoolYear { get; set; }

        public required string Name { get; set; }

        public required string DivisionState { get; set; }

        public Schedule? Schedule { get; set; }
    }
}
