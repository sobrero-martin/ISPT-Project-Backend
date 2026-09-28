using BD.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace BD.Entidades
{
    public class SchoolYear : BaseEntity
    {
        public int SchoolYearNumber { get; set; }
        public ICollection<SchoolYearCurriculum> SchoolYearCurriculums { get; set; } = new List<SchoolYearCurriculum>();
    }
}
