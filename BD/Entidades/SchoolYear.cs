using System;
using System.Collections.Generic;
using System.Text;

namespace BD.Entidades
{
    public class SchoolYear : BaseEntity
    {
        public long CurriculumID { get; set; }
        public Curriculum? Curriculum { get; set; }

        public int SchoolYearNumber { get; set; }
    }
}
