using BD.Entidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace BD.Entities
{
    public class SchoolYearCurriculum : BaseEntity
    {
        public long CurriculumId { get; set; }
        public Curriculum? Curriculum { get; set; }
        public long SchoolYearId { get; set; }
        public SchoolYear? SchoolYear { get; set; }
        public int SchoolYearNumber { get; set; }
    }
}
